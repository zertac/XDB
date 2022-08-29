using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace XDB
{
    public static class XDBConfigurator
    {
        public static string ConnectionString;
        public static Dictionary<string, Map> Maps;
        public static bool UseCache { get; set; }
        public static long CacheTimeout { get; set; }

        static XDBConfigurator()
        {
            Maps = new Dictionary<string, Map>();
        }

        public static void Init(LoadType type, string db, string folder = null)
        {
            LoadQueries(type, db, folder);
        }

        public static Exception SetConnection(string conString)
        {
            if (string.IsNullOrEmpty(conString))
            {
                return new Exception("Empty connection string");
            }

            ConnectionString = conString;

            return null;
        }

        private static void LoadQueries(LoadType type, string db, string folder = null)
        {
            if (type == LoadType.PROCEDURE)
            {
                var mp = new Map();
                mp.query = "select * from mysql.proc where db = 'fuathoca_development' AND type = 'PROCEDURE'";
                AddMap("load_query", mp);


                var res = (List<Dictionary<string, object>>)Main.ExecuteCommand("load_query", null);

                for (int i = 0; i < res.Count; i++)
                {
                    var r = res[i];
                    var map = new Map();
                    var prms = Encoding.Default.GetString(r["param_list"] as byte[] ?? throw new InvalidOperationException());
                    map.query = CreateQueryString(r["specific_name"].ToString(), prms);
                    map.fields = CreateMapFields(prms);
                    AddMap(r["specific_name"].ToString(), map);
                }
            }
            else if (type == LoadType.FOLDER)
            {
                string[] filePaths = Directory.GetFiles(folder, "*.sql");

                foreach (var path in filePaths)
                {
                    var key = Path.GetFileNameWithoutExtension(path);
                    var value = File.ReadAllText(path);

                    var m = new Map();
                    m.query = value;
                    AddMap(key, m);
                }
            }
        }

        private static string CreateQueryString(string query, string prms)
        {
            var b = new StringBuilder();
            b.Append("CALL `");
            b.Append(query);
            b.Append("`(");
            b.Append(CreateParametersString(prms));
            b.Append(");");
            return b.ToString();
        }

        private static string CreateParametersString(string prms)
        {
            var str = prms.Trim();

            var tmpLst = str.Split(',');
            var b = new StringBuilder();
            for (int i = 0; i < tmpLst.Length; i++)
            {
                var r = tmpLst[i];
                if (!String.IsNullOrEmpty(r))
                {
                    var p = r.Split(' ')[1];
                    b.Append('@');
                    b.Append(p);
                    b.Append(",");
                }
            }
            b.Append("#");

            return b.ToString().Replace(",#", "").Replace("#", "");
        }

        private static List<MapField> CreateMapFields(string prms)
        {
            var str = prms.Trim();

            var tmpLst = str.Split(',');
            var lst = new List<MapField>();

            for (int i = 0; i < tmpLst.Length; i++)
            {
                var r = tmpLst[i];
                if (!String.IsNullOrEmpty(r))
                {
                    var p = r.Split(' ')[1];
                    var field = new MapField();
                    field.key = "@" + p;
                    lst.Add(field);
                }
            }

            return lst;
        }

        public static Exception AddMap(string mapName, Map map)
        {
            if (!Maps.ContainsKey(mapName)) Maps.Add(mapName, map);
            else return new Exception("This map already added for this connection type !");

            return null;
        }

        public static Map GetMap(string mapKey)
        {
            var mapStr = Maps[mapKey];

            if (mapStr == null)
            {
                return null;
            }

            return Maps[mapKey];
        }

        public static List<DbParam> CreateParameters(string mapKey, Dictionary<string, object> data)
        {
            if (data == null || data.Count == 0) return new List<DbParam>();

            var lst = new List<DbParam>();

            var map = GetMap(mapKey);

            if (map == null)
            {
                throw new Exception("Map not found");
            }

            for (int i = 0; i < map.fields.Count; i++)
            {
                if (data.ContainsKey(map.fields[i].value))
                {
                    var p = new DbParam();
                    p.key = map.fields[i].key;
                    p.value = data[map.fields[i].value];
                    lst.Add(p);
                }
            }

            return lst;
        }
    }
}
