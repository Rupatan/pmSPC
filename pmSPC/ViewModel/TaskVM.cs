using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pmSPC.Model;
using pmSPC.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static pmSPC.ViewModel.WorkStationVM;

namespace pmSPC.ViewModel
{
    public class TaskVM : ViewModelBase<TaskModel>
    {

        private List<Position> listPositions = App.ListPosition;

        public List<Position> ListPositions
        {
            get { return listPositions; }
            set
            {
                listPositions = value;
                OnPropertyChanged("ListPositions");
            }
        }

        private Position selectedPosition;

        public Position SelectedPosition
        {
            get { return selectedPosition; }
            set
            {
                selectedPosition = value;
                OnPropertyChanged("SelectedPosition");
            }
        }



        public TaskVM() : base("update")
        {
        }

        private TaskModel objectModel = new TaskModel();

        public TaskModel ObjectModel
        {
            get { return objectModel; }
            set
            {
                objectModel = value;
                OnPropertyChanged("ObjectModel");
            }
        }

        private RelayCommand saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (
                  saveCommand = new RelayCommand(SaveCommandHandler)
            );
            }
        }

        private RelayCommand closeCommand;

        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand ?? (
                  closeCommand = new RelayCommand(obj =>
                  {
                      View.TaskView window = (View.TaskView)obj;
                      window.Close();
                  }
                  )
              );
            }
        }

        public void SaveCommandHandler(object obj)
        {
            string stringJSON = JsonConvert.SerializeObject(ObjectModel);
            GetJSONAsync((objJson) => RequestPrc((String)objJson, (obj1) =>
            {
                NotifyChanged("save", objectModel.Id);
                ((Window)obj).Close();
            }, NotifyUser), stringJSON);
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

        public void NotifyUser(object obj)
        {
            string info = ((JObject)obj)["info"]?.Value<string>();
            if (!info.isEmpty())
                MessageBox.Show(info, "Предупреждение");
        }

        protected override void ResponseHandler(string stringJSON)
        {
            RequestPrc(stringJSON, (objJson) =>
            {
                JObject obj = (JObject)objJson;
                string taskModelString = obj["data"]?.Value<JToken>().ToString();
                if (!string.IsNullOrEmpty(taskModelString))
                {
                    TaskModel taskModel = JsonConvert.DeserializeObject<TaskModel>(taskModelString);
                    if (taskModel.Id != null)
                    {
                        objectModel.Date = taskModel.Date;
                        objectModel.Id = taskModel.Id;
                        objectModel.Number = taskModel.Number;
                    }
                }
            });
        }
    }


}

