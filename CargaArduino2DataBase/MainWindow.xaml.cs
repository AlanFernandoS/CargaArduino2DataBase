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
            //TablaDeRegistrosSerial.AutoGenerateColumns = true;
            //TablaDeRegistrosSerial.ItemsSource = ListaDeEventosSeriales;
            //TablaDeDatos.ItemsSource = ListaDeEventos;
            //MyTimer2Update.AutoReset = true;
            //MyTimer2Update.Interval = (500);
            //MyTimer2Update.Elapsed += MyTimer2Update_Elapsed;
            //MyTimer2Update.Start();
        }

        //private void MyTimer2Update_Elapsed(object sender, ElapsedEventArgs e)
        //{

        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        try
        //        {
        //            TablaDeDatos.Items.Refresh();
        //            TablaDeRegistrosSerial.Items.Refresh();
        //        }
        //        catch
        //        {

        //        }
        //        //Monitor.Enter(ListaDeEventosSeriales);
        //        //TablaDeRegistrosSerial.Items.DeferRefresh();
        //        //Monitor.Exit(ListaDeEventosSeriales);
        //        //Monitor.Enter(ListaDeEventos);
        //        //TablaDeDatos.Items.DeferRefresh();
        //        //Monitor.Exit(ListaDeEventos);
        //    }));


        //}

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
            foreach (string cm in Comandos)
            {
                var Trimmed = cm.Trim();
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

                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    //    try
                    //    {
                    //        TablaDeDatos.Items.Refresh();
                    //        TablaDeRegistrosSerial.Items.Refresh();
                    //    }
                    //    catch
                    //    {

                    //    }

                    //}));

                }
            }

            try
            {

                //var ComandoPorSerial = JsonConvert.DeserializeObject<List<ComandoArduino>>(indata);
                //foreach (ComandoArduino Cm in ComandoPorSerial)
                //{
                //    EventosEnElDT.Push(Cm);
                //}
                //ConvertCilindricalOnList();
            }
            catch (SystemException ER)
            {
                MessageBox.Show(ER.Message.ToString());
            }
            //Console.WriteLine("Data Received:");
            //Console.Write(indata);
        }
        //private void ConvertCilindricalOnList()
        //{
        //    ListaDeEventos.Clear();
        //    for (int i = 0; i < 30; i++)
        //    {
        //        ListaDeEventos.Add(EventosEnElDT.Array[i]);
        //    }
        //    TablaDeDatos.Items.Refresh();
        //}

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
    public class CircularArray<T>
    {
        private readonly T[] _baseArray;
        private readonly T[] _facadeArray;
        private int _head;
        private bool _isFilled;

        public CircularArray(int length)
        {
            _baseArray = new T[length];
            _facadeArray = new T[length];
        }

        public T[] Array
        {
            get
            {
                int pos = _head;
                for (int i = 0; i < _baseArray.Length; i++)
                {
                    Math.DivRem(pos, _baseArray.Length, out pos);
                    _facadeArray[i] = _baseArray[pos];
                    pos++;
                }
                return _facadeArray;
            }
        }

        public T[] BaseArray
        {
            get { return _baseArray; }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
        }

        public void Push(T value)
        {
            if (!_isFilled && _head == _baseArray.Length - 1)
                _isFilled = true;

            Math.DivRem(_head, _baseArray.Length, out _head);
            _baseArray[_head] = value;
            _head++;
        }

        public T Get(int indexBackFromHead)
        {
            int pos = _head - indexBackFromHead - 1;
            pos = pos < 0 ? pos + _baseArray.Length : pos;
            Math.DivRem(pos, _baseArray.Length, out pos);
            return _baseArray[pos];
        }
    }
}

