using pmSPC.Controls;
using pmSPC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pmSPC.View
{
    /// <summary>
    /// Логика взаимодействия для TaskView.xaml
    /// </summary>
    public partial class TaskView : Window
    {
        public TaskView()
        {
            InitializeComponent();
        }

        void TextBoxDropDownHintControl_OnSelect(object sender, SelectionChanged e)
        {
            if (e.Value != null)
            {
                Position model = e.Value as Position;
                if (model == null) return;

                //tbxId.Text = model.Id.ToString();
                //tbxName.Text = model.Name;
                //tbxDescription.Text = model.Description;
            }
        }

    }
}
