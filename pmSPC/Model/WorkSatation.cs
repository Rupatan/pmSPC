using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using pmSPC.Other;

namespace pmSPC.Model
{
    public class WorkSatation : ModelBase
    {

        public static string PATHHTTP = "{0:S}/ws/".format(App.URL);

        private string _code, _contragent, _name;
        [JsonProperty("Код")]
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                OnPropertyChanged("Code");
            }
        }

        [JsonProperty("Контрагент")]
        public string Contragent
        {
            get { return _contragent; }
            set
            {
                _contragent = value;
                OnPropertyChanged("Contragent");
            }
        }

        [JsonProperty("РабочееМесто")]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

        private string commentAdministrator;
        [JsonIgnore()]
        public string CommentAdministrator
        {
            get { return commentAdministrator; }
            set
            {
                commentAdministrator = value;
                OnPropertyChanged("CommentAdministrator");
            }
        }

        private string commentUser;

        [JsonIgnore()]
        public string CommentUser
        {
            get { return commentUser; }
            set
            {
                commentUser = value;
                OnPropertyChanged("CommentUser");
            }
        }

        private bool isCriticalData;
        [JsonIgnore()]
        public bool IsCriticalData
        {
            get { return isCriticalData; }
            set
            {
                isCriticalData = value;
                OnPropertyChanged("IsCriticalData");
            }
        }
    }
}
