using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace pmSPC.Other
{
    class ModelContext
    {

        public static string getIdComputer()
        {
            List<String> array = new List<string> { Environment.MachineName };

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
            {
                foreach (ManagementObject obj in searcher.Get())
                    array.Add(obj.GetPropertyValue("SerialNumber").ToString());

                searcher.Query = new ObjectQuery("SELECT * FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                    array.Add(obj.GetPropertyValue("ProcessorId").ToString());

            }
            return MD5ToString(String.Join(";", array).GetHashCode().ToString());
        }

        public static string MD5ToString(string s)
        {
            using (var alg = MD5.Create())
                return string.Join(null, alg.ComputeHash(Encoding.UTF8.GetBytes(s)).Select(x => x.ToString("x2")));
        }

        public static string getURLService(string action, params object[] args)
        {
            return getURL(App.URL, action, args);
        }

        public static string getURL(string URL, string action, params object[] args)
        {
            string u = URL;
            if (u[u.Length - 1] != '/')
                u = "{0}/".format(u);

            u = "{0}{1}".format(u, action);

            if (args.Length > 0)
                u = "{0}?{1}".format(u, urlStringParams(args));

            return u;

        }

        public static string urlStringParams(params object[] args)
        {

            return String.Join("&", args.Select(arg =>
            {
                Type type = arg.GetType();
                return String.Join("&", type.GetProperties().Select(prop => "{0}={1}".format(prop.Name, prop.GetValue(arg).ToString())).ToArray());
            }).ToArray());
        }

        public static async Task<string> SendJSONAsync(string json, string action)
        {
            var result = await sendServiceAsync(ModelContext.getURLService(action, new { id = App.computerId }), json);

            return result;
        }

        public static async Task<String> sendServiceAsync(string URLService, string body = "", string Method = "POST")
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URLService);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Credentials = new NetworkCredential(App.Login, App.Password);
            req.Method = Method;
            if (!body.isEmpty())
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                    w.Write(body);

            string outString = "";
            using (HttpWebResponse resp = await req.GetResponseAsync() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    outString = sr.ReadToEnd();
            }

            return outString;
        }

        public static async Task<string> sendServiceAsync(string URLService, Action<string> p, string body = "", string Method = "POST")
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URLService);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Credentials = new NetworkCredential(App.Login, App.Password);
            req.Method = Method;

            if (!body.isEmpty())
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                    w.Write(body);

            string outString = "";
            using (HttpWebResponse resp = await req.GetResponseAsync() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    outString = sr.ReadToEnd();
            }

            if (p != null)
                await Application.Current.Dispatcher.BeginInvoke(p, outString);

            return outString;
        }

        public static string SendJSON(string json, string action)
        {
            var result = sendService(ModelContext.getURLService(action, new { id = App.computerId }), json);

            return result;
        }

        public static String sendService(string URLService, string body = "", string Method = "POST")
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URLService);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Credentials = new NetworkCredential(App.Login, App.Password);
            req.Method = Method;
            if (!body.isEmpty())
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                    w.Write(body);

            string outString = "";
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    outString = sr.ReadToEnd();
            }

            return outString;
        }

    }
}
