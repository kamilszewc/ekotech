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

namespace Ekotech
{
    /// <summary>
    /// Interaction logic for AddPomiarWindow.xaml
    /// </summary>
    public partial class AddPomiarWindow : Window
    {
        Pomiar _pomiar;

        public AddPomiarWindow(Pomiar pomiar)
        {
            InitializeComponent();

            _pomiar = pomiar;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _pomiar.Time = this.czasSpin.Value.ToString();
            this.DialogResult = true;
            this.Close();
        }
    }
}
