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
        List<ComandoArduino> ListaDeEventos = new List<ComandoArduino>();
        List<string> ListaDeEventosSeriales = new List<string>();
        List<DataComplete> ReporteDeComandos = new List<DataComplete>();
        Int32 NumRegSend = 0;
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
                if ( BotonConectar.Content.Equals("Conectar"))
                {
                    if (ComboPuerto.Text != "No hay puertos disponibles")
                    {
                        PuertoSerial = new SerialPort();
                        PuertoSerial.PortName = ComboPuerto.Text;
                        PuertoSerial.BaudRate = Convert.ToInt32(ComboVelocidad.Text);
                        PuertoSerial.ReadTimeout = (100);
                        PuertoSerial.Open();
                        ListaDeEventosSeriales.Clear();
                        Monitor.Enter(ReporteDeComandos);
                        ReporteDeComandos.Clear();
                        Monitor.Exit(ReporteDeComandos);
                        //Monitor.Enter(NumRegSend);
                        //NumRegSend = 0;
                        //Monitor.Exit(NumRegSend);
                        NumRegSend = 0;
                        NumeroDeRegistrosEnviados.Text = NumRegSend.ToString();
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
                PuertoSerial.Close();
                MessageBox.Show(_Error.Message.ToString());
            }
        }

        private void PuertoSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadLine();
                var Trimmed = indata.Trim();
                if (Trimmed.Length > 0 && Trimmed.StartsWith("{"))
                {
                    var NuevoRegistro = new DataComplete();
                    try
                    {
                        NuevoRegistro.RecivedData = Trimmed;
                        var CmS = JsonConvert.DeserializeObject<ComandoArduino>(Trimmed);
                        NuevoRegistro.ArduinoComand = CmS;
                        var MySqlCon = new AdmSQL(CmS.IP, CmS.DB, CmS.User, CmS.Pass);
                        switch (CmS.CMD)
                        {
                            case "Insert":
                                var Infodata = CmS.Data.Replace("\\\"", "\"");
                                var Info2Load = JsonConvert.DeserializeObject<SortedList<String, String>>(Infodata);
                                var ResponseSql = MySqlCon.InsertaSQLSpecificColums(CmS.Table, Info2Load);
                                CmS.Data = Infodata.Replace("@ServerDT", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                                NuevoRegistro.Error = ResponseSql;
                                //What i need to do here to put NumRegSend++, without get Miscount error
                                //Dispatcher.BeginInvoke(new Action(() =>
                                //{
                                //    NumeroDeRegistrosEnviados.Text = (ReporteDeComandos.Count+1).ToString();
                                //}),9);//Normal priority
                                break;
                        }
                    }
                    catch (Exception MyExcepcion)
                    {
                        NuevoRegistro.Error = MyExcepcion.Message.ToString();
                    }
                    Monitor.Enter(ReporteDeComandos);
                    ReporteDeComandos.Add(NuevoRegistro);
                    Monitor.Exit(ReporteDeComandos);
                }
            }
            catch
            {

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var Detalle = new DatosYComandos(ReporteDeComandos);
            Detalle.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PuertoSerial.Close();
            MessageBox.Show("Que tengas un excelente día");
        }
    }
    public class ComandoArduino
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
    public class DataComplete
    {
        public string   RecivedData { get; set; } = "";
        public ComandoArduino ArduinoComand { get; set; } = new ComandoArduino();
        public string Error { get; set; } = "";
    }
 
}

