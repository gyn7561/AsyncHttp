using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AsyncHttp.Entity
{
    public class HttpHeaders : ICloneable
    {
        private List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

        public ISet<String> Names()
        {
            return new HashSet<String>(list.Select(kv => kv.Key), StringComparer.OrdinalIgnoreCase);
        }

        public List<KeyValuePair<string, string>> Headers()
        {
            return (this.Clone() as HttpHeaders).list;
        }

        public List<KeyValuePair<string, string>> GetAllValue(string key)
        {
            return list.Where(kv => String.Compare(key, kv.Key, true) == 0).ToList();
        }

        public bool ContainsKey(string key)
        {
            return this.Names().Contains(key);
        }

        public string this[string key]
        {
            get
            {
                foreach (var kv in list)
                {
                    if (String.Compare(kv.Key, key, true) == 0)
                    {
                        return kv.Value;
                    }
                }
                return null;
            }
            set
            {
                var newKv = new KeyValuePair<string, string>(key, value);
                for (int i = 0; i < list.Count; i++)
                {
                    var kv = list[i];
                    if (String.Compare(kv.Key, key, true) == 0)
                    {
                        list[i] = newKv;
                    }
                }
                list.Add(newKv);
            }
        }

        public object Clone()
        {
            var clone = new HttpHeaders();
            this.list.ForEach(kv =>
            {
                clone[kv.Key] = kv.Value;
            });
            return clone;
        }

        public string ToHttpCommandString()
        {
            var commandBuilder = new StringBuilder();
            foreach (var kv in this.list)
            {
                commandBuilder.Append($"{kv.Key}: {kv.Value}\r\n");
            }
            return commandBuilder.ToString();
        }
    }
}
