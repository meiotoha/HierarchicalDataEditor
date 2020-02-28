using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HierarchicalDataEditor.Core.Helpers
{
    public static class ObjectHelper
    {
        public static object TryGetFirstOrDefaultValue(this object[] context, string name)
        {
            if (context == null)
            {
                return default;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return default;
            }

            var propertyName = name.Replace('{', ' ').Replace('}', ' ').Trim();
            if (propertyName == ".")
            {
                return context.FirstOrDefault();
            }
            foreach (var obj in context)
            {
                if (obj is Hashtable ht)
                {
                    if (ht.ContainsKey(propertyName))
                    {
                        return ht[propertyName];
                    }
                }
                else
                {
                    var value = obj.TryGetValue(propertyName);
                    if (value != null)
                    {
                        return value;
                    }
                }

             
            }
            return default;
        }

        private static object TryGetValue(this object obj, string name)
        {
            var type = obj.GetType();
            var property = type.GetProperty(name);
            return property != null ? property.GetValue(obj) : default;
        }
    }
}
