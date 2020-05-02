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
using System.Net;
using System.Net.NetworkInformation;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using Newtonsoft.Json;
namespace CargaArduino2DataBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort PuertoSerial = new SerialPort();
        CircularArray<ComandoArduino> EventosEnElDT = new CircularArray<ComandoArduino>(30);
        List<ComandoArduino> ListaDeEventos = new List<ComandoArduino>();
        List<string> ListaDeEventosSeriales = new List<string>();
        //System.Timers.Timer MyTimer2Update = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            BotonActualizar.Click += BotonActualizar_Click;
            BotonConectar.Click += BotonConectar_Click;
            ActualizaPuertos();

        }



        private void BotonConectar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BotonConectar.Content == "Conectar")
                {
                    if (ComboPuerto.Text != "No hay puertos disponibles")
                    {
                        PuertoSerial = new SerialPort();
                        PuertoSerial.PortName = ComboPuerto.Text;
                        PuertoSerial.BaudRate = Convert.ToInt32(ComboVelocidad.Text);
                        PuertoSerial.ReadTimeout = (100);
                        PuertoSerial.Open();
                        ListaDeEventosSeriales.Clear();
                        PuertoSerial.DataReceived += PuertoSerial_DataReceived;
                        BotonConectar.Content = "Desconectar";
                    }

                }
                else
                {
                    PuertoSerial.Close();
                    BotonConectar.Content = "Conectar";
                }
            }
            catch (System.Exception _Error)
            {
                MessageBox.Show(_Error.Message.ToString());
            }
        }

        private void PuertoSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            //var Comandos = indata.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            var Trimmed = indata.Trim();
            if (Trimmed.Length > 0 && Trimmed.StartsWith("{"))
            {
                try
                {
                    Monitor.Enter(ListaDeEventosSeriales);
                    ListaDeEventosSeriales.Add(Trimmed);
                    Monitor.Exit(ListaDeEventosSeriales);
                    var CmS = JsonConvert.DeserializeObject<ComandoArduino>(Trimmed);
                    Monitor.Enter(ListaDeEventos);
                    ListaDeEventos.Add(CmS);
                    Monitor.Exit(ListaDeEventos);
                }
                catch
                {
                }
            }

        }

        private void BotonActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                ActualizaPuertos();
            }
        }
        private void ActualizaPuertos()
        {
            var PuertosDisponibles = SerialPort.GetPortNames();
            if (PuertosDisponibles.ToList<String>().Count == 0)
            {
                PuertosDisponibles = new string[] { "No hay puertos disponibles" };
            }
            ComboPuerto.ItemsSource = PuertosDisponibles;
            ComboPuerto.SelectedIndex = 0;
        }
    }
    class ComandoArduino
    {
        private string dB = "";
        private string user = "";
        private string pass = "";
        private string cMD = "";
        private string table = "";
        private string data = "";

        public string IP { get; set; } = "";
        public string DB { get => dB; set => dB = value; }
        public string User { get => user; set => user = value; }
        public string Pass { get => pass; set => pass = value; }
        public string CMD { get => cMD; set => cMD = value; }
        public string Table { get => table; set => table = value; }
        public string Data { get => data; set => data = value; }
    }
}

