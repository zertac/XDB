using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace XDB
{
    internal static class XCacher
    {
        public static ConcurrentDictionary<string, CacheContainer> Values;

        private static Timer _timer;

        static XCacher()
        {
            Values = new ConcurrentDictionary<string, CacheContainer>();

            _timer = new Timer { Interval = 5000, Enabled = true };
            _timer.Elapsed += Clean;
            _timer.Enabled = true;
            _timer.Start();
        }

        public static void SetValue(string key, object value, double minute)
        {
            var c = new CacheContainer();
            c.Expire = DateTimeOffset.Now.AddMinutes(minute).ToUnixTimeSeconds();
            c.Value = value;

            if (Values.ContainsKey(key))
            {
                Values[key] = c;
            }
            else
            {
                Values.TryAdd(key, c);
            }
        }

        public static T GetObject<T>(string key)
        {
            if (Values.ContainsKey(key))
            {
                if (Values[key].Value != null)
                {
                    return (T)Values[key].Value;
                }
            }

            return default;
        }

        public static void Remove(string key)
        {
            if (Values.ContainsKey(key))
            {
                Values.TryRemove(key, out _);
            }
        }

        public static void Flush()
        {
            Values = new ConcurrentDictionary<string, CacheContainer>();
        }

        public static void Clean(object sender, System.Timers.ElapsedEventArgs e)
        {
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();

            var list = Values.ToList();
            var removeList = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value != null)
                {
                    if (now >= list[i].Value.Expire)
                    {
                        removeList.Add(list[i].Key);
                    }
                }
            }

            for (int i = 0; i < removeList.Count; i++)
            {
                Values.TryRemove(removeList[i], out _);
            }
        }
    }
}
