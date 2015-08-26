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
using System.Globalization;
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace Ekotech
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Pomiar> _pomiary;
        public double _g = 981.0; // cm/s^2

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            _pomiary = new List<Pomiar>();
            this.listBox.ItemsSource = _pomiary;
        }

        private void DodajPomiar_Click(object sender, RoutedEventArgs e)
        {
            Pomiar pomiar = new Pomiar("0.0", "0.0");
            AddPomiarWindow addPomiarWindow = new AddPomiarWindow(pomiar) { Owner = this };


            if (addPomiarWindow.ShowDialog() == true)
            {
                double time = Convert.ToDouble(pomiar.Time);
                double mass = (double)this.mass.Value;
                double height = (double)this.length.Value;
                double power = (mass * _g * height / time)/10000.0;
                if (time == 0.0) power = 0.0;

                _pomiary.Add(new Pomiar(time.ToString(), power.ToString("F2")));
                this.listBox.ItemsSource = null;
                this.listBox.ItemsSource = _pomiary;

                if (_pomiary.Count > 0)
                {
                    this.buttonUsunPomiar.IsEnabled = true;
                    this.mass.IsEnabled = false;
                    this.massError.IsEnabled = false;
                    this.length.IsEnabled = false;
                    this.lengthError.IsEnabled = false;
                    this.buttonZapisz.IsEnabled = true;
                }
                else
                {
                    this.buttonUsunPomiar.IsEnabled = false;
                    this.mass.IsEnabled = true;
                    this.massError.IsEnabled = true;
                    this.length.IsEnabled = true;
                    this.lengthError.IsEnabled = true;
                    this.buttonZapisz.IsEnabled = false;
                }

                Calculate();
            }
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
                this.buttonZapisz.IsEnabled = true;
            }
            else
            {
                this.buttonUsunPomiar.IsEnabled = false;
                this.mass.IsEnabled = true;
                this.massError.IsEnabled = true;
                this.length.IsEnabled = true;
                this.lengthError.IsEnabled = true;
                this.buttonZapisz.IsEnabled = false;
            }

            Calculate();
        }

        private void Calculate()
        {
            double mass = (double)this.mass.Value;
            double height = (double)this.length.Value;
            double massError = (double)this.massError.Value;
            double heightError = (double)this.lengthError.Value;

            List<double> pomiary = new List<double>();
            foreach (var pomiar in _pomiary)
            {
                double time = Convert.ToDouble(pomiar.Time);
                pomiary.Add(time);
            }

            double averageTime = 0.0;
            double errorTime = 0.0;

            foreach (double pomiar in pomiary)
            {
                averageTime += pomiar;
            }
            averageTime = averageTime / pomiary.Count;

            foreach (double pomiar in pomiary)
            {
                errorTime += Math.Pow(pomiar - averageTime, 2);
            }
            errorTime = 3.0 * Math.Sqrt(errorTime / (pomiary.Count - 1) ) / Math.Sqrt( (double)pomiary.Count );

            double errorPowerMass = (_g * height / averageTime) * massError;
            double errorPowerHeight = (mass * _g / averageTime) * heightError;
            double errorPowerTime = (-mass * _g * height / Math.Pow(averageTime, 2)) * errorTime;

            double error = (Math.Abs(errorPowerMass) + Math.Abs(errorPowerHeight) + Math.Abs(errorPowerTime))/10000.0;

            double average = (mass * _g * height / averageTime)/10000.0;

            string errorString = error.ToString();
            string errorNewString = "";

            Console.WriteLine(errorString);
            for (int i = 0; i < errorString.Length; i++)
            {
                errorNewString += errorString[i];
                if (errorString[i] == 'N') { errorNewString="---"; break; }
                if ((errorString[i] != '.') && (errorString[i] != ',') && (errorString[i] != '0')) break;
            }

            string averageNewString = "";
            if (errorNewString != "---")
            {
                double errorNewDouble = Convert.ToDouble(errorNewString);
                if ((errorNewDouble / average) > 0.1)
                {
                    errorNewString += errorString[errorNewString.Length];
                }

                string averageString = average.ToString();
                
                for (int i = 0; i < errorNewString.Length; i++)
                {
                    averageNewString += averageString[i];
                }
            }
            else
            {
                averageNewString = "---";
            }


            

            this.power.Content = averageNewString;
            this.error.Content = errorNewString;

        }

        private void ButtonZapisz_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file (*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == true)
            {
                double mass = (double)this.mass.Value;
                double height = (double)this.length.Value;
                double massError = (double)this.massError.Value;
                double heightError = (double)this.lengthError.Value;

                string data = ""; 
                data += "Masa = " + mass.ToString() + " +- " + massError.ToString() + " g \r\n";
                data += "Dlugosc = " + height.ToString() + " +- " + heightError.ToString() + " cm \r\n";
                data += "\r\n";
                data += "Pomiary czasu i mocy:" + "\r\n";

                foreach (Pomiar pomiar in _pomiary)
                {
                    data += "Czas = " + pomiar.Time + " s  " + "Moc = " + pomiar.Power + " mW \r\n"; 
                }
                data += "\r\n";

                data += "Moc skuteczna = " + this.power.Content + " +- " + this.error.Content + " mW \r\n";

                File.WriteAllText(dialog.FileName, data);
            }
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
