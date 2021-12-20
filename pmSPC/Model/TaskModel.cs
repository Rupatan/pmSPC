using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pmSPC.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pmSPC.Model
{
    public class TaskModel : ModelBase
    {

        public static string PATHHTTP = "{0:S}/tasks/".format(App.URL);

        private string _id, _name, _number;
        private DateTime _date;
        private double leadTime;

        [JsonProperty("Ссылка")]
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        [JsonProperty("Дата")]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        [JsonProperty("Номер")]
        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        [JsonProperty("Наименование")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string comment;
        [JsonProperty("Комментарий")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
            }
        }


        [JsonProperty("Факт")]
        public double LeadTime
        {
            get { return leadTime; }
            set
            {
                leadTime = value;
                OnPropertyChanged("LeadTime");
            }
        }

        private Position executor;

        [JsonProperty("Исполнитель")]
        public Position Executor
        {
            get { return executor; }
            set
            {
                executor = value;
                OnPropertyChanged("Executor");
            }
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        private bool haveTask = false;

        [JsonProperty("ЕстьЗадача")]
        public bool HaveTask 
        {
            get { return haveTask; }
            set
            {
                haveTask = value;
                OnPropertyChanged("HaveTask");
            }
        }

        private string textSubtask;

        [JsonProperty("ТекстПодзадачи")]
        public string TextSubtask
        {
            get { return textSubtask; }
            set
            {
                textSubtask = value;
                HaveTask = !textSubtask.isEmpty();
                OnPropertyChanged("TextSubtask");
            }
        }
    }
}
