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
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data;
using System.Reflection;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using System.Resources;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft;
using System.IO;
namespace CargaArduino2DataBase
{
    /// <summary>
    /// Interaction logic for DatosYComandos.xaml
    /// </summary>
    public partial class DatosYComandos : Window
    {
        List<MuestraDeDatosOnGrid> ListaDeComandosParaExportar = new List<MuestraDeDatosOnGrid>();
        List<SortedList<String, String>> Data2Show = new List<SortedList<String, String>>();
        DataTable TablaParaExportar = new DataTable();
        bool ShowTable = true;
        public DatosYComandos(List<DataComplete> ParametrosRecibidosYProcesados)
        {
            InitializeComponent();
            if (ParametrosRecibidosYProcesados.Count > 100)
            {
                MessageBox.Show("La del detalle es muy larga para mostrarse, te recomendamos exportarla directamente");
                ShowTable = false;
            }
            //ParametrosRecibidos = ParametrosRecibidosYProcesados;
            foreach (DataComplete _Reg in ParametrosRecibidosYProcesados)
            {
                var MDG = new MuestraDeDatosOnGrid();
                MDG.ComandoSerial = _Reg.RecivedData;
                MDG.CMD = _Reg.ArduinoComand.CMD;
                MDG.IP = _Reg.ArduinoComand.IP;
                MDG.DB = _Reg.ArduinoComand.DB;
                MDG.Table = _Reg.ArduinoComand.Table;
                MDG.Data = _Reg.ArduinoComand.Data;
                MDG.Error = _Reg.Error;
                if (MDG.Error.Length == 0)
                {
                    var Infodata = _Reg.ArduinoComand.Data.Replace("\\\"", "\"");
                    var Info2Load = JsonConvert.DeserializeObject<SortedList<String, String>>(Infodata);
                    Data2Show.Add(Info2Load);
                }
                ListaDeComandosParaExportar.Add(MDG);
            }
            TablaParaExportar = ToDataTable<MuestraDeDatosOnGrid>(ListaDeComandosParaExportar);
            if (ShowTable)
            {
                TablaDeRegistrosObtenidos.ItemsSource = TablaParaExportar.DefaultView;
            }
        }
        public class MuestraDeDatosOnGrid
        {
            public string ComandoSerial { get; set; } = "";
            public string IP { get; set; } = "";
            public string DB { get; set; } = "";
            public string CMD { get; set; } = "";
            public string Table { get; set; } = "";
            public string Data { get; set; } = "";
            public string Error { get; set; } = "";
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private void BotonSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void InsertDataTableIntoExcelDocument(ref SLDocument excelDoc, DataTable Table2Insert, int Row, int Column, bool WriteHeaders)
        {
            var ListaDeHeaders = new List<string>();
            foreach (DataColumn _Col in Table2Insert.Columns)
                ListaDeHeaders.Add(_Col.ColumnName);
            int InitialColumnIndex = Column;
            int MaxColumnIndex = Column + ListaDeHeaders.Count - 1;
            if (WriteHeaders)
            {
                for (int Index = InitialColumnIndex, loopTo = MaxColumnIndex; Index <= loopTo; Index++)
                    excelDoc.SetCellValue(Row, Index, ListaDeHeaders[Index - InitialColumnIndex]);
                Row = Row + 1;
            }

            foreach (DataRow _Row in Table2Insert.Rows)
            {
                for (int Index = InitialColumnIndex, loopTo1 = MaxColumnIndex; Index <= loopTo1; Index++)
                {
                    int IndexDeHeader = Index - InitialColumnIndex;
                    string SeleccionDeColumna = ListaDeHeaders[IndexDeHeader];
                    var ReadValue = _Row[SeleccionDeColumna];
                    if (ReadValue == null | ReadValue == DBNull.Value)
                    {
                        ReadValue = "";
                    }
                    excelDoc.SetCellValue(Row, Index, ReadValue.ToString());
                }

                Row = Row + 1;
            }
        }

        private void BotonExportar_Click(object sender, RoutedEventArgs e)
        {
            if (RadioExcel.IsChecked == true)
            {
                var excelD = new SLDocument();
                SLStyle StyleHeaders = excelD.CreateStyle();
                StyleHeaders.Fill.SetPattern(PatternValues.Solid,
                                      System.Drawing.Color.FromArgb(184, 204, 228),
                                      System.Drawing.Color.FromArgb(184, 204, 228)
                                      );
                StyleHeaders.SetFontBold(true);
                InsertDataTableIntoExcelDocument(ref excelD, TablaParaExportar, 1, 1, true);
                for (int i = 1; i < TablaParaExportar.Columns.Count + 1; i++)
                {
                    excelD.SetCellStyle(1, i, StyleHeaders);
                    if (i != 1)
                    {
                        excelD.AutoFitColumn(i);
                    }
                }
                var TablaCMD = excelD.CreateTable(1, 1, TablaParaExportar.Rows.Count, TablaParaExportar.Columns.Count);
                TablaCMD.HasTotalRow = false;
                TablaCMD.HasAutoFilter = true;
                TablaCMD.DisplayName = "TablaDeComandos";
                excelD.InsertTable(TablaCMD);
                excelD.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Comandos");
                
                excelD.AddWorksheet("Datos");
                //Busqueda de columnas
                var ListaDeColumnas = new List<string>();
                var OnlyDataT = new DataTable();
                Data2Show.ForEach(delegate (SortedList<String, String> _Registro)
                {
                    var ColumnasNoAgregadas = (from V in _Registro.Keys.ToList<string>() where !ListaDeColumnas.Contains(V) select V).ToList();
                    ColumnasNoAgregadas.ForEach(delegate (string Cname)
                    {
                        ListaDeColumnas.Add(Cname);
                        OnlyDataT.Columns.Add(Cname);
                    });
                });
                //Procesando la información para generalizar los casos mas comunes
                Data2Show.ForEach(delegate (SortedList<String, String> _Registro)
                {
                    var NuevaFila = OnlyDataT.NewRow();
                    _Registro.Keys.ToList<string>().ForEach(
                        delegate (string Cname)
                        {
                            NuevaFila[Cname] = _Registro[Cname];
                        });
                    OnlyDataT.Rows.Add(NuevaFila);
                });
                InsertDataTableIntoExcelDocument(ref excelD, OnlyDataT, 1, 1, true);
                for (int i = 1; i < OnlyDataT.Columns.Count + 1; i++)
                {
                    excelD.SetCellStyle(1, i, StyleHeaders);
                    excelD.AutoFitColumn(i);
                }
                var Tabla=excelD.CreateTable(1,1,OnlyDataT.Rows.Count,OnlyDataT.Columns.Count);
                Tabla.HasTotalRow = false;
                Tabla.HasAutoFilter = true;
                Tabla.DisplayName = "TablaDeDatos";
                excelD.InsertTable(Tabla);
                try
                {
                    excelD.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ReporteDeCargaArduino2DB.xlsx");
                    MessageBox.Show("Se ha exportado correctamente el reporte al escritorio con el nombre de ReporteDeCargaArduino2DB");
                }
                catch (Exception MyE)
                {
                    MessageBox.Show("Se ha detectado un error al crear el documento de excel, " + MyE.Message.ToString());
                }
            }
            if (RadioCSV.IsChecked==true)
            {
                ToCSV(TablaParaExportar, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CsvReporteDeCarga.csv");
                //Busqueda de columnas
                var ListaDeColumnas = new List<string>();
                var OnlyDataT = new DataTable();
                Data2Show.ForEach(delegate (SortedList<String, String> _Registro)
                {
                    var ColumnasNoAgregadas = (from V in _Registro.Keys.ToList<string>() where !ListaDeColumnas.Contains(V) select V).ToList();
                    ColumnasNoAgregadas.ForEach(delegate (string Cname)
                    {
                        ListaDeColumnas.Add(Cname);
                        OnlyDataT.Columns.Add(Cname);
                    });
                });
                //Procesando la información para generalizar los casos mas comunes
                Data2Show.ForEach(delegate (SortedList<String, String> _Registro)
                {
                    var NuevaFila = OnlyDataT.NewRow();
                    _Registro.Keys.ToList<string>().ForEach(
                        delegate (string Cname)
                        {
                            NuevaFila[Cname] = _Registro[Cname];
                        });
                    OnlyDataT.Rows.Add(NuevaFila);
                });
                ToCSV(OnlyDataT, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CsvData.csv");
            }
        }

        private void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);
                //headers  
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (System.Exception ErrorCSV)
            {
                MessageBox.Show("Error al exportar los archivos, " + ErrorCSV.Message.ToString());
            }
            finally
            {
                MessageBox.Show("Se han exportado correctamete los archivos en el escritorio, con los nombres de CsvData.csv y CsvReporteDeCarga.csv");
            }
        }
        //private void ToCSV(this DataTable dtDataTable, string strFilePath)
        //{
        //    StreamWriter sw = new StreamWriter(strFilePath, false);
        //    //headers  
        //    for (int i = 0; i < dtDataTable.Columns.Count; i++)
        //    {
        //        sw.Write(dtDataTable.Columns[i]);
        //        if (i < dtDataTable.Columns.Count - 1)
        //        {
        //            sw.Write(",");
        //        }
        //    }
        //    sw.Write(sw.NewLine);
        //    foreach (DataRow dr in dtDataTable.Rows)
        //    {
        //        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        //        {
        //            if (!Convert.IsDBNull(dr[i]))
        //            {
        //                string value = dr[i].ToString();
        //                if (value.Contains(','))
        //                {
        //                    value = String.Format("\"{0}\"", value);
        //                    sw.Write(value);
        //                }
        //                else
        //                {
        //                    sw.Write(dr[i].ToString());
        //                }
        //            }
        //            if (i < dtDataTable.Columns.Count - 1)
        //            {
        //                sw.Write(",");
        //            }
        //        }
        //        sw.Write(sw.NewLine);
        //    }
        //    sw.Close();
        //}

    }
     
}
