using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pmSPC.Other;
using pmSPC.Model;
using System.IO;

namespace pmSPC
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static object lockObject = new object();
        public static string computerId = Other.ModelContext.getIdComputer();
        //public static readonly string URL = "http://95.79.48.85:8008/Torg83_Debug_DK/hs/pmSPC";
        public static readonly string URL = "http://95.79.48.85:8008/Torg83_debug_Alehin/hs/pmSPC";
        //public static readonly string URL = "http://95.79.48.85:8008/Torg83/hs/pmSPC";
        public static WorkSatation workStation;
        public static string Login = "pmSPC";
        public static string Password = "Yggghjv1002!!!";

        //public static string Login = "Backup";
        //public static string Password = "123";

        public static List<Position> ListPosition { get; set; }

        public App()
        {
            workStation = new WorkSatation();

            //var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));

            String path = AppDomain.CurrentDomain.BaseDirectory;
        }



    }
}
