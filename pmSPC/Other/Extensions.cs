using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmSPC.Other
{
    public static class Extensions
    {
        public static string format(this string str, params string[] args)
        {
            return String.Format(str, args);
        }

        public static bool isEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static object getValue(this object arg, string name)
        {
            var props = arg.GetType().GetProperties();
            return props.Where(prop => prop.Name.Contains(name)).First().GetValue(arg);
        }

        public static string getJSON(this object args)
        {
            Type t = args.GetType();

            Dictionary<String, Object> result = t.GetProperties().Select(prop => new { name = prop.Name, value = prop.GetValue(args) }).ToDictionary(k => k.name, v => v.value);

            return JsonConvert.SerializeObject(result);
        }
    }

}
