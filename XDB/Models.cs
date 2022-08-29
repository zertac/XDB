using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDB
{
    public enum LoadType
    {
        FOLDER = 0,
        PROCEDURE = 1
    }

    public class DbParam
    {
        public string key;
        public object value;
    }

    public class Map
    {
        public string query { get; set; }
        public List<MapField> fields { get; set; }
    }

    public class MapField
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    internal class CacheContainer
    {
        public long Expire { get; set; }
        public object Value { get; set; }
    }

    public class DbError
    {
        public int Error { get; set; }
        public string Description { get; set; }
    }
}
