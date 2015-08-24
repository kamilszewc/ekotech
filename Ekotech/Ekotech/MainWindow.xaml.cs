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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Ekotech
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Pomiar> _pomiary;
        public double _mass = 0.0;
        public double _massError = 0.0;
        public double _height = 0.0;
        public double _heightError = 0.0;
        public double _g = 981.0; // cm/s^2

        public MainWindow()
        {
            InitializeComponent();

            _pomiary = new List<Pomiar>();
            this.listBox.ItemsSource = _pomiary;
        }

        private void DodajPomiar_Click(object sender, RoutedEventArgs e)
        {
            double time = 0.0;
            double power = _mass * _g * _height / time;
            if (time == 0.0) power = 0.0;

            _pomiary.Add(new Pomiar(time.ToString(), power.ToString()));
            this.listBox.ItemsSource = null;
            this.listBox.ItemsSource = _pomiary;

            if (_pomiary.Count > 0)
            {
                this.buttonUsunPomiar.IsEnabled = true;
                this.mass.IsEnabled = false;
                this.massError.IsEnabled = false;
                this.length.IsEnabled = false;
                this.lengthError.IsEnabled = false;
            }
            else
            {
                this.buttonUsunPomiar.IsEnabled = false;
                this.mass.IsEnabled = true;
                this.massError.IsEnabled = true;
                this.length.IsEnabled = true;
                this.lengthError.IsEnabled = true;
            }

            Calculate();
        }

        private void buttonUsunPomiar_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.listBox.SelectedIndex;

            try
            {
                _pomiary.RemoveAt(selectedIndex);
            }
            catch
            { }

            this.listBox.ItemsSource = null;
            this.listBox.ItemsSource = _pomiary;

            if (_pomiary.Count > 0)
            {
                this.buttonUsunPomiar.IsEnabled = true;
                this.mass.IsEnabled = false;
                this.massError.IsEnabled = false;
                this.length.IsEnabled = false;
                this.lengthError.IsEnabled = false;
            }
            else
            {
                this.buttonUsunPomiar.IsEnabled = false;
                this.mass.IsEnabled = true;
                this.massError.IsEnabled = true;
                this.length.IsEnabled = true;
                this.lengthError.IsEnabled = true;
            }

            Calculate();
        }

        private void Calculate()
        {
        }
    }

    public class Pomiar
    {
        public string Time { set; get; }
        public string Power { set; get; }

        public Pomiar(string time, string power)
        {
            Time = time;
            Power = power;
        }
    }
}
