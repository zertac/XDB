using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using XDB.Connectors;

namespace XDB
{
    public static class Main
    {
        public static object ExecuteCommand(string mapKey, Dictionary<string, object> values)
        {
            var dt = new XDBMySqlConnector(XDBConfigurator.ConnectionString);
            var result = dt.ExecCommand(mapKey, XDBConfigurator.CreateParameters(mapKey, values));

            return result;
        }

        public static T GetData<T>(string command, Dictionary<string, object> parameters = null, Action<DbError> OnError = null)
        {
            var cache = XDBConfigurator.UseCache;

            if (!cache)
            {
                return CrawlData<T>(command, parameters, OnError);
            }

            var data = GetDataByCache<T>(command);

            if (data == null)
            {
                data = CrawlData<T>(command, parameters, OnError);
                XCacher.SetValue(command, data, XDBConfigurator.CacheTimeout);
            }

            return default;
        }

        private static T CrawlData<T>(string command, Dictionary<string, object> parameters = null, Action<DbError> OnError)
        {
            var res = (List<Dictionary<string, object>>)ExecuteCommand(command, parameters);

            var type = typeof(T);
            var isList =
                   type.IsGenericType &&
                   type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));

            if (res.Count > 0)
            {
                if ((res[0].ContainsKey("ERROR") == false) || (res[0].ContainsKey("ERROR") && res[0]["ERROR"] != null && Convert.ToInt32(res[0]["ERROR"]) == 0))
                {
                    if (!isList)
                    {
                        if (res.Count > 0)
                        {
                            var tmp = JsonConvert.SerializeObject(res[0]);
                            return JsonConvert.DeserializeObject<T>(tmp);
                        }

                        return default;
                    }


                    var tmp2 = JsonConvert.SerializeObject(res);
                    return JsonConvert.DeserializeObject<T>(tmp2);
                }
                else
                {
                    if (OnError != null)
                    {
                        var error = new DbError()
                        {
                            Error = Convert.ToInt32(res[0]["ERROR"]),
                            Description = res[0].ContainsKey("DESCRIPTION") ? res[0]["DESCRIPTION"]?.ToString() : ""
                        };

                        OnError(error);
                    }
                }
            }
            else
            {
                if (OnError != null)
                {
                    var error = new DbError()
                    {
                        Error = -1,
                        Description = "No data found"
                    };

                    OnError(error);
                }
            }

            return default;
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        private static T GetDataByCache<T>(string key)
        {
            var data = XCacher.GetObject<T>(key);

            if (data == null)
            {
                return default;
            }

            return data;
        }

        public static void FlushCache()
        {
            XCacher.Flush();
        }
    }
}
