using pmSPC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pmSPC.Other;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace pmSPC.ViewModel
{

    public class TaskListVM : ViewModelBase<TaskModel>
    {
        public Window window;

        private List<TaskModel> listTask = new List<TaskModel>();


        private TaskModel selectedItem;

        public TaskModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("selectedItem");
            }
        }

        private RelayCommand refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return refreshCommand ?? (
                    refreshCommand = new RelayCommand(obj =>
                    {
                        GetJSONAsync();
                    }));
            }
        }

        private RelayCommand editTaskCommand;
        public RelayCommand EditTaskCommand
        {
            get
            {
                return editTaskCommand ?? (
                    editTaskCommand = new RelayCommand(obj =>
                    {
                        if (obj == null)
                            return;

                        View.TaskView window = new View.TaskView();
                        window.Owner = Application.Current.MainWindow;
                        TaskVM task = window.Resources["model"] as TaskVM;
                        
                        task.EventNotifyViewModel += Task_EventNotifyViewModel;

                        string stringTask = JsonConvert.SerializeObject(obj);
                        TaskModel taskModelNew = JsonConvert.DeserializeObject<TaskModel>(stringTask);
                        task.ObjectModel = taskModelNew;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        window.ShowDialog();
                    }
                    )
                );
            }
        }

        private void Task_EventNotifyViewModel(string nameEvent, object result)
        {
            if (nameEvent.ToLower().Equals("save"))
            {
                string id = (string)result;
                listTask.Remove(listTask.Where(x => x.Id.Equals(id)).FirstOrDefault());

                ListTask = new List<TaskModel>(listTask);                
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        public List<TaskModel> ListTask
        {
            get { return listTask; }
            set
            {
                listTask = value;
                OnPropertyChanged("ListTask");
            }
        }

        public TaskListVM() : base("getlist")
        {

        }

        protected override void ResponseHandler(string stringJSON)
        {
            JObject obj = (JObject)JsonConvert.DeserializeObject(stringJSON);
            var taskJson = obj.Value<JObject>("data").Value<JArray>("Задачи");
            ListTask.Clear();
            if (taskJson.Count > 0)
            {
                string tasksString = taskJson.ToString();
                ListTask = JsonConvert.DeserializeObject<List<TaskModel>>(tasksString);
            }
        }

        public void CallbackResponse(object obj)
        {
            GetJSONAsync();
        }

    }
}
