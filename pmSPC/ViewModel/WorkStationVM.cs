using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pmSPC.Model;
using pmSPC.Other;


namespace pmSPC.ViewModel
{
    public class WorkStationVM : Model.WorkSatation
    {

        public delegate void CallbackEventHandler(object obj);

        public event CallbackEventHandler CallBackEvent;

        public delegate void CallbackHttpCompletedEvent(Action<string> action);

        public Window window;

        private bool isFound = false;

        public bool IsFound
        {
            get { return isFound; }
            set
            {
                isFound = value;
                OnPropertyChanged("IsFound");

                SendButtonName = isFound ? "Отменить" : "Зарегистрировать";
            }
        }

        private bool isShow = false;

        public bool IsShow
        {
            get { return isShow; }
            set
            {
                isShow = value;
                OnPropertyChanged("IsShow");
            }
        }

        private RelayCommand sendCommand;
        public RelayCommand SendCommand
        {
            get
            {
                return sendCommand ?? (
                    sendCommand = new RelayCommand(SendCode)
                );
            }
        }

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (
                    cancelCommand = new RelayCommand(CancelCommandHandler,
                    (o => { return isFound; })
                    )
                );
            }
        }

        private RelayCommand registerCommand;
        public RelayCommand RegisterCommand
        {
            get
            {
                return registerCommand ?? (
                    registerCommand = new RelayCommand(RegisterComponent)
                );
            }
        }

        private string sendButtonName = "Зарегистрировать";

        public string SendButtonName
        {
            get { return sendButtonName; }
            set
            {
                sendButtonName = value;
                OnPropertyChanged("SendButtonName");
            }
        }


        public async void CancelCommandHandler(object obj)
        {
            string url = ModelContext.getURLService("ws/cancelbinding", new { id = App.computerId });
            await ModelContext.sendServiceAsync(url, CancelCommandHandlerEnd);
        }

        public void CancelCommandHandlerEnd(object obj)
        {
            RequestPrc(obj.ToString(), (objJson) =>
            {
                IsFound = false;

                MessageBox.Show(((JObject)objJson)["info"]?.Value<string>(), "Оповещение");

            });

        }

        public WorkStationVM()
        {
            InitializeWorkStation();
        }

        public void RequestPrc(string stringJson, Action<object> action)
        {
            JObject result = (JObject)JsonConvert.DeserializeObject(stringJson);
            int status = result["status"]?.Value<int>() ?? 0;
            if (status == 1)
                action.Invoke(result);

        }

        public void RequestPrc(string stringJson, Action<object> actionTrue, Action<object> actionFalse)
        {
            ResultResponse resultResponse = JsonConvert.DeserializeObject<ResultResponse>(stringJson);
            JObject result = (JObject)JsonConvert.DeserializeObject(stringJson);
            int status = resultResponse.Status ?? 0;
            if (status == 1)
                actionTrue.Invoke(result);
            else
                actionFalse.Invoke(result);

        }

        public void SendCodeQuestion(string stringJSON)
        {
            RequestPrc(stringJSON, SendQuestionPrc, NotifyUser);
        }

        public void NotifyUser(object obj)
        {
            string info = ((JObject)obj)["info"]?.Value<string>();
            if (!info.isEmpty())
                MessageBox.Show(info, "Предупреждение");
        }

        public async void SendQuestionPrc(object obj)
        {
            JObject jObject = (JObject)obj;

            JToken objJson = jObject["data"]?.Value<JToken>();

            Operation operation = JsonConvert.DeserializeObject<Operation>(jObject["operation"]?.Value<JToken>().ToString());
            if (operation.Name == "find")
            {
                WorkSatation workSatation = JsonConvert.DeserializeObject<WorkSatation>(objJson.ToString());
                if (workSatation != null &&
                    MessageBox.Show("Привязка будет установлена для контрагента \"{0}\" (рабочее место \"{1}\")".format(workSatation.Contragent, workSatation.Name), "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string url = ModelContext.getURLService("ws/register", new { id = App.computerId }),
                        bodyJson = new { code = Code }.getJSON();

                    await ModelContext.sendServiceAsync(url, (strJson) =>
                    {
                        RequestPrc(strJson, (objectJson) => WSResultHandler((JObject)objectJson), NotifyUser);
                    }, bodyJson);

                }
            }
            else if (operation.Name.Equals("cancelbinding"))
            {
                MessageBox.Show(jObject["info"]?.Value<string>(), "Оповещение");
                IsFound = false;
            }

        }

        public async void SendCode(object obj)
        {
            string url, bodyJson = "";
            if (!IsFound)
            {
                url = "ws/find";
                bodyJson = new { ПараметрыПоиска = new { Код = Code } }.getJSON();
            }
            else url = "ws/cancelbinding";
            url = ModelContext.getURLService(url, new { id = App.computerId });
            await ModelContext.sendServiceAsync(url, SendCodeQuestion, bodyJson);
        }

        public async void RegisterComponent(object obj = null)
        {
            List<QueryComponent> ListQueryComponent = await GetQueryComponent();

            List<Component> listComponent = new List<Component>();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher())
            {
                foreach (QueryComponent qc in ListQueryComponent)
                {
                    if (qc.WIN32_Class.isEmpty())
                        continue;

                    searcher.Query = new ObjectQuery($"SELECT * FROM {qc.WIN32_Class}");
                    foreach (ManagementObject info in searcher.Get())
                    {
                        Component component = new Component();
                        component.Id = qc.Id;
                        component.WIN32_Class = qc.WIN32_Class;
                        component.Property = GetComponentInfo(info, qc.Property);
                        listComponent.Add(component);
                    }

                }
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("id", App.computerId);
            dict.Add("code", Code);
            dict.Add("component", listComponent);
            dict.Add("commentadministrator", CommentAdministrator);
            dict.Add("commentuser", CommentUser);
            dict.Add("iscriticaldata", IsCriticalData);

            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            string stringJSON = await ModelContext.SendJSONAsync(json, "component");

            //WSResultHandler(stringJSON);
        }


        public List<Property> GetComponentInfo(ManagementObject obj, List<Property> property)
        {
            List<Property> dict = new List<Property>();
            foreach (Property p in property)
            {
                try
                {
                    Property info = new Property { Id = p.Id, Value = obj[p.Value.ToString()] };
                    dict.Add(info);
                }
                catch (Exception ex)
                {
                    Property info = new Property { Id = p.Id, Value = ex.Message };
                    dict.Add(info);
                }
            }
            return dict;
        }

        public async void InitializeWorkStation()
        {
            string url = ModelContext.getURLService("ws/getWS", new { id = App.computerId });
            await ModelContext.sendServiceAsync(url, WSResultHandler);

        }

        public void WSResultHandler(JObject result)
        {
            IsShow = true;
            int status = result["status"]?.Value<int>() ?? 0;
            if (status == 1)
            {
                var data = result["data"]?.Value<JToken>();
                if (data != null)
                {
                    App.workStation = JsonConvert.DeserializeObject<WorkSatation>(data.ToString());

                    string jsonUsers = data["Пользователи"]?.Value<JArray>().ToString();
                    if (jsonUsers.isEmpty() == false)
                        App.ListPosition = JsonConvert.DeserializeObject<List<Position>>(jsonUsers);


                    Code = App.workStation.Code;
                    Contragent = App.workStation.Contragent;
                    Name = App.workStation.Name;

                    CallBackEvent?.Invoke(this);

                    IsFound = true;
                }
            }
        }

        public void WSResultHandler(string stringJSON)
        {
            JObject result = (JObject)JsonConvert.DeserializeObject(stringJSON);
            WSResultHandler(result);
        }


        public async Task<List<QueryComponent>> GetQueryComponent()
        {
            var stringJSON = await ModelContext.SendJSONAsync("", "qc");
            return JsonConvert.DeserializeObject<List<QueryComponent>>(stringJSON);
        }

        public class Property
        {
            public string Id { get; set; }
            public object Value { get; set; }

        }
        public class QueryComponent
        {
            public string Id { get; set; }
            public string WIN32_Class { get; set; }

            public List<Property> Property { get; set; }

        }
        public class Component
        {
            public string Id { get; set; }
            public string WIN32_Class { get; set; }

            public List<Property> Property { get; set; }

        }

        public class Operation
        {
            public string Name { get; set; }

            public bool? Completed { get; set; }
        }

        public class ResultResponse
        {
            [JsonProperty("status")]
            public int? Status { get; set; }

            [JsonProperty("info")]
            public string Info { get; set; }

            [JsonProperty("operation")]
            public Operation InfoOperation { get; set; }

        }

    }
}
