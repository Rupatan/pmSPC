using pmSPC.Other;
using pmSPC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace pmSPC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)sender;
            foreach (var r in window.Resources)
                r.GetType().GetProperties().Where(x => x.Name.ToLower().Equals("window")).FirstOrDefault()?.SetValue(r, window);

            TaskListVM taskListVM = (TaskListVM)window.Resources["Task"];
            ((WorkStationVM)window.Resources["ws"]).CallBackEvent += taskListVM.CallbackResponse;

        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
