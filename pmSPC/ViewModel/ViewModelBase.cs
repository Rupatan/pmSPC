using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pmSPC.Model;
using pmSPC.Other;

namespace pmSPC.ViewModel
{



    public abstract class ViewModelBase<T> : Model.ModelBase
    {

        public delegate void NotifyViewModel(string nameEvent, object result);

        public event NotifyViewModel EventNotifyViewModel;

        public void NotifyChanged(string nameEvent, object result)
        {
            if (EventNotifyViewModel != null)
                EventNotifyViewModel.Invoke(nameEvent, result);
        }

        public enum Method
        {
            POST,
            GET
        }

        protected Type type;
        protected string URLBase;
        protected string func;

        public ViewModelBase(string method)
        {
            this.func = method;
            this.type = typeof(T);
            URLBase = GetURLBase();
        }

        protected string GetURLBase()
        {
            FieldInfo[] props = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            FieldInfo prop = props.Where(w => w.Name.ToLower() == "pathhttp").FirstOrDefault<FieldInfo>();
            string path = (string)prop?.GetValue(type);
            return Other.ModelContext.getURL(path, func, new { id = App.computerId });
        }

        public async void GetJSONAsync(string body = "", Method method = Method.POST)
        {
            string methodString = method.ToString();
            string stringJson = await ModelContext.sendServiceAsync(URLBase, ResponseHandler, body, methodString);
        }

        public async void GetJSONAsync(Action<object> action, string body = "", Method method = Method.POST)
        {
            string methodString = method.ToString();
            string stringJson = await ModelContext.sendServiceAsync(URLBase, action, body, methodString);
        }
        public void RequestPrc(string stringJson, Action<object> action)
        {
            JObject result = (JObject)JsonConvert.DeserializeObject(stringJson);
            int status = result["status"]?.Value<int>() ?? 0;
            if (status == 1)
                action.Invoke(result);

        }
        protected abstract void ResponseHandler(string stringJSON);

    }
}
