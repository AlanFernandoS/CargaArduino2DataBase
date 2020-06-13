using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
namespace CargaArduino2DataBase
{
    class AdmSQL

    {
        protected string ConnectionString = "";

        // "Data Source = 192.168.0.100;Initial Catalog=ManProd;Persist Security Info=True;User ID=usuarios;Password=1234 "
        public AdmSQL(string MyIp, string BaseD, string Usu, string Pwd)
        {
            // ConnectionString = "Data Source = localhost; Initial Catalog=" + BaseD + ";Integrated Security=True"
            ConnectionString = "Data Source = " + MyIp + ";Initial Catalog=" + BaseD + ";Persist Security Info=True;User ID=" + Usu + ";Password=" + Pwd;
        }

        public AdmSQL(string InOneLineSpScp, ref bool CompError)
        {
            string[] MyData = InOneLineSpScp.Split('\n');
            if (MyData.Count() == 4)
                ConnectionString = "Data Source = " + MyData[0] + ";Initial Catalog=" + MyData[1] + ";Persist Security Info=True;User ID=" + MyData[2] + ";Password=" + MyData[3];
            else
                CompError = true;
        }

        /// <summary>
        ///     ''' Permite retornar el string para la conexión creado en base al constructor de la clase.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string RetornaElConnectionString()
        {
            return ConnectionString;
        }

        /// <summary>
        ///     ''' Permite de una manera tener las comparaciones necesarias para relizar el cambio o actualizacion
        ///     ''' regresa una lista de manera que quedan las columnas igualadas de esta manera Columna(n)='Lis(n)'
        ///     ''' </summary>
        ///     ''' <param name="Columna"></param>
        ///     ''' <param name="Lis2"></param>
        ///     ''' <returns></returns>
        public List<string> RetornaIgualdades(List<string> Columna, List<string> Lis2)
        {
            List<string> Devuelta = new List<string>();
            if (Columna.Count == Lis2.Count)
            {
                for (int index = 0; index <= Columna.Count - 1; index++)
                    Devuelta.Add(Columna[index] + " = '" + Lis2[index] + "'");
            }
            else
            {

            }
                //("Error las listas no son del mismo tamaño");
            return Devuelta;
        }


        public List<string> RetornaIgualdadesV2(List<string> Columna, List<string> Lis2, ref bool IndicadorDeError)
        {
            IndicadorDeError = false;
            List<string> Devuelta = new List<string>();
            if (Columna.Count == Lis2.Count)
            {
                for (int index = 0; index <= Columna.Count - 1; index++)
                    Devuelta.Add(Columna[index] + " = '" + Lis2[index] + "'");
            }
            else
                IndicadorDeError = true;
            return Devuelta;
        }
        /// <summary>
        ///     ''' Columa = 'DatoToCompare' Nota: No Añade un espacio en blanco al final
        ///     ''' </summary>
        ///     ''' <param name="Columna"></param>
        ///     ''' <param name="DatoToCompare"></param>
        ///     ''' <returns></returns>
        public string RetornaIgualdadesV2(string Columna, string DatoToCompare)
        {
            return Columna + " = " + InsertComillas(DatoToCompare);
        }

        public List<String> RetornaIgualdadesSinComillas(List<string> Columna, List<string> Lis2)
        {
            List<string> Devuelta = new List<string>();
            if (Columna.Count == Lis2.Count)
            {
                for (int index = 0; index <= Columna.Count - 1; index++)
                    Devuelta.Add(Columna[index] + " = " + Lis2[index] + " ");
            }
            return Devuelta;
        }

        /// <summary>
        ///     ''' Retorna el string de consulta basado en
        ///     ''' Select C1,C2,...,Cn From TablaSQL Where Cond1 Or/And Cond2 ...Or/And CondN
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="lColum"></param>
        ///     ''' <param name="CondBusqueda"></param>
        ///     ''' <param name="Condicionante"></param>
        ///     ''' <returns></returns>
        public string ArmaConSql(string TablaSQL, List<string> lColum, List<string> CondBusqueda, List<string> Condicionante)
        {
            var StrConsulta = "Select ";
            var MaxIndex = lColum.Count - 1;
            try
            {
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        StrConsulta = StrConsulta + lColum[index];
                    else
                        StrConsulta = StrConsulta + lColum[index] + ", ";
                }
                StrConsulta = StrConsulta + " From " + TablaSQL + " Where ";

                MaxIndex = CondBusqueda.Count - 1;
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        StrConsulta = StrConsulta + CondBusqueda[index];
                    else
                        StrConsulta = StrConsulta + CondBusqueda[index] + Condicionante[index] + " ";
                }
            }
            catch (Exception er)
            {
                //Interaction.MsgBox(er.Message, Title: "Módulo de armado de consulta condicionada");
                return "";
            }
            return StrConsulta.Trim();
        }

        /// <summary>
        ///     ''' Permite estructurar una consulta condicionada
        ///     ''' Select colum1,colum2,...,columnN form TablaSQL where Con1 and Cond2 ... and CondN
        ///     ''' </summary>
        ///     ''' <param name="lColum"></param>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="CondBusqueda"></param>
        ///     ''' <returns></returns>
        public string ArmaConSql(string TablaSQL, List<string> lColum, List<string> CondBusqueda)
        {
            var StrConsulta = "Select ";
            var MaxIndex = lColum.Count - 1;
            try
            {
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        StrConsulta = StrConsulta + lColum[index];
                    else
                        StrConsulta = StrConsulta + lColum[index] + ", ";
                }
                StrConsulta = StrConsulta + " From " + TablaSQL + " Where ";

                MaxIndex = CondBusqueda.Count - 1;
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        StrConsulta = StrConsulta + CondBusqueda[index];
                    else
                        StrConsulta = StrConsulta + CondBusqueda[index] + "And ";
                }
            }
            catch (Exception er)
            {
                //Interaction.MsgBox(er.Message, Title: "Módulo de armado de consulta condicionada");
            }
            return StrConsulta.Trim();
        }

        /// <summary>
        ///     ''' Estructura una busqueda de manera que select * from TablaSQL where Cond1 and Cond2 ... and ConN
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="CondBusqueda"></param>
        ///     ''' <returns></returns>
        public string ArmaConSql(string TablaSQL, List<string> CondBusqueda)
        {
            var StrConsulta = "Select * From " + TablaSQL + " Where ";
            var MaxIndex = CondBusqueda.Count - 1;
            try
            {
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        StrConsulta = StrConsulta + CondBusqueda[index];
                    else
                        StrConsulta = StrConsulta + CondBusqueda[index] + " And ";
                }
            }
            catch (Exception er)
            {
                //Interaction.MsgBox("Ha habido un error en la busqueda condicionada" + Constants.vbCrLf + er.Message + Constants.vbCrLf + StrConsulta, Title: "Error en el módulo armaSQL");
            }
            return StrConsulta.Trim();
        }

        /// <summary>
        ///     ''' Retorna Select * from TablaSQL
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <returns></returns>
        public string ArmaConSQL(string TablaSQL)
        {
            return "Select * From " + TablaSQL;
        }

        /// <summary>
        ///     ''' Simplificada para el uso de simplemente el String de condicion
        ///     ''' Select * from TablaSQL where CondBusqueda
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="CondBusqueda"></param>
        ///     ''' <returns></returns>
        public string ArmaConSql(string TablaSQL, string CondBusqueda)
        {
            var StrConsulta = "Select * From " + TablaSQL + " Where ";
            try
            {
                StrConsulta = StrConsulta + CondBusqueda;
            }
            catch (Exception er)
            {
                //Interaction.MsgBox("Ha habido un error en ArmaSql" + Constants.vbCrLf + er.Message, Title: "Error en el módulo armaSQL");
            }
            return StrConsulta.Trim();
        }
        public string ConcatenaDataSortedList2Update(SortedList<string, string> Data)
        {
            var MaxIndexRegister = Data.Keys.Count - 1;
            var ConcatenacionData = "";
            // Dim BaseOfChange As String = " @Columna='@Data' "
            for (int IndexRegister = 0; IndexRegister <= MaxIndexRegister; IndexRegister++)
            {
                if (IndexRegister == MaxIndexRegister)
                    ConcatenacionData += " " + Data.Keys[IndexRegister] + "= '" + Data.Values[IndexRegister] + "' ";
                else
                    ConcatenacionData += " " + Data.Keys[IndexRegister] + "= '" + Data.Values[IndexRegister] + "', ";
            }
            return ConcatenacionData;
        }

        /// <summary>
        ///     ''' Dim ComandoSql = "Update @TableSql  Set @NewValues Where @Conditions"
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="Condiciones"></param>
        ///     ''' <param name="NewData"></param>
        ///     ''' <returns></returns>
        public string UpdateOnSQL(string TablaSQL, List<string> Condiciones, SortedList<string, string> NewData)
        {
            var ComandoSql = "Update @TableSql  Set @NewValues Where @Conditions";
            string ErrorSql = "";
            ComandoSql = ComandoSql.Replace("@TableSql", TablaSQL);

            string SqlCondition = JoinTheConditionsClauses(Condiciones);
            ComandoSql = ComandoSql.Replace("@Conditions", SqlCondition);

            var ConcatenacionData = ConcatenaDataSortedList2Update(NewData);
            ComandoSql = ComandoSql.Replace("@NewValues", ConcatenacionData);

            ComandoSql = ComandoSql.Replace("'@NULL'", "NULL");

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ComandoSql, con);
                    int Respuesta = cmd.ExecuteNonQuery();
                    if (Respuesta == 0)
                        ErrorSql = "Error: No se ha logrado registrar la informacion en la base de datos: " + ComandoSql;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorSql = "Error:" +  "\n" + ex.Message +  "\n" + "ComandoSql: " + ComandoSql;
            }
            return ErrorSql;
        }

        // -------------Funciones de UPDATE de la informacion
        // UPDATE table_name
        // Set column1 = value1, column2 = value2, ...
        // WHERE condition;
        /// <summary>
        ///     ''' Realizara una instrucción basada en
        ///     ''' Update TablaSQL set C1 ='CambiosSQL1', C2 ='CambiosSQL2',..., CN ='CambiosSQLN' Where Con1 And Con2 And Con3.
        ///     ''' Devuelve un true en caso de ser exitosa la actualización
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="CambiosSQL"></param>
        ///     ''' <param name="Condiciones"></param>
        public List<string> UpdateOnSQL(string TablaSQL, List<string> CambiosSQL, List<string> Condiciones)
        {
            string Comando = "Update " + TablaSQL + " set ";
            List<string> ListaDeErrores = new List<string>();
            try
            {
                var MaxIndex = CambiosSQL.Count - 1;
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        Comando = Comando + CambiosSQL[index];
                    else
                        Comando = Comando + CambiosSQL[index] + ", ";
                }
                Comando = Comando + " where ";
                MaxIndex = Condiciones.Count - 1;
                for (int index = 0; index <= MaxIndex; index++)
                {
                    if (index == MaxIndex)
                        Comando = Comando + Condiciones[index];
                    else
                        Comando = Comando + Condiciones[index] + " and ";
                }
                // Until here are the preparations------------------------
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Comando, con);
                    var respuesta = cmd.ExecuteNonQuery();
                    if (respuesta == 0)
                        // Exito = False
                        ListaDeErrores.Add("No se ha modificado ningun registro por alguna extraña razón, nota del programador." +  "\n" + Comando);
                    con.Close();
                }
            }
            catch (Exception er)
            {
                ListaDeErrores.Add(er.Message);
            }
            return ListaDeErrores;
        }
        public async Task<List<string>> UpdateOnSQLAsync(string TablaSQL, List<string> CambiosSQL, List<string> Condiciones)
        {
            string Comando = "Update " + TablaSQL + " set ";
            List<string> ListaDeErrores = new List<string>();
            try
            {
                var ComandoA = await JoinNewDataInUpdateClauseAsync(CambiosSQL);
                var ComandoB = await JoinTheConditionsClausesAsync(Condiciones);

                Comando = Comando + ComandoA;
                Comando = Comando + ComandoB + ";";
                // Until here are the preparations------------------------
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Comando, con);
                    var respuesta = cmd.ExecuteNonQuery();
                    if (respuesta == 0)
                        // Exito = False
                        ListaDeErrores.Add("No se ha modificado ningun registro por alguna extraña razón, nota del programador." +  "\n" + Comando);
                    con.Close();
                }
            }
            catch (Exception er)
            {
                ListaDeErrores.Add(er.Message);
            }
            return ListaDeErrores;
        }
        public async Task<string> JoinNewDataInUpdateClauseAsync(List<string> CambiosSQL)
        {
            string Comando = "";
            var MaxIndex = CambiosSQL.Count - 1;
            for (int index = 0; index <= MaxIndex; index++)
            {
                if (index == MaxIndex)
                    Comando = Comando + CambiosSQL[index];
                else
                    Comando = Comando + CambiosSQL[index] + ", ";
            }
            return Comando;
        }
        public async Task<string> JoinTheConditionsClausesAsync(List<string> Condiciones)
        {
            string Comando = "";
            var MaxIndex = Condiciones.Count - 1;
            for (int index = 0; index <= MaxIndex; index++)
            {
                if (index == MaxIndex)
                    Comando = Comando + Condiciones[index];
                else
                    Comando = Comando + Condiciones[index] + " and ";
            }
            return Comando;
        }
        /// <summary>
        ///     ''' Cond1 and Cond2 ...and CondN
        ///     ''' </summary>
        ///     ''' <param name="Condiciones"></param>
        ///     ''' <returns></returns>
        public string JoinTheConditionsClauses(List<string> Condiciones)
        {
            string Comando = "";
            var MaxIndex = Condiciones.Count - 1;
            for (int index = 0; index <= MaxIndex; index++)
            {
                if (index == MaxIndex)
                    Comando = Comando + Condiciones[index];
                else
                    Comando = Comando + Condiciones[index] + " and ";
            }
            return Comando;
        }
        public string JoinNewDataInUpdateClause(List<string> CambiosSQL)
        {
            string Comando = "";
            var MaxIndex = CambiosSQL.Count - 1;
            for (int index = 0; index <= MaxIndex; index++)
            {
                if (index == MaxIndex)
                    Comando = Comando + CambiosSQL[index];
                else
                    Comando = Comando + CambiosSQL[index] + ", ";
            }
            return Comando;
        }
        /// <summary>
        ///     ''' Examina en la Tabla SQL en base a la consulta y para acelerar el proceso solo se toma una columna, si esta existe existe el resto
        ///     ''' para ello es necesario recuperar el contenido de la columna en base a un tipo propuesto.
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <param name="MyColumn"></param>
        ///     ''' <param name="MyType"></param>
        ///     ''' <returns></returns>
        public bool ExisteLaconsulta(string TablaSQL, string Consulta, string MyColumn, object MyType)
        {
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    cmd.Parameters.AddWithValue("@" + MyColumn, MyType);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        existe = true;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                //Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Error");
            }
            return existe;
        }
        public bool ExisteLaconsultaV2(string TablaSQL, string Consulta, ref string AreSomeErrorInside)
        {
            AreSomeErrorInside = "";
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        existe = true;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AreSomeErrorInside = ex.Message;
            }
            return existe;
        }
        public bool ExisteLaconsultaV2(string Consulta, ref string AreSomeErrorInside)
        {
            AreSomeErrorInside = "";
            bool existe = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        existe = true;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AreSomeErrorInside = ex.Message;
            }
            return existe;
        }
        /// <summary>
        ///     ''' Regresa un string que dice
        ///     ''' Existe si la clave fue encontrada
        ///     ''' No existe, si la clave no fue encontrada
        ///     ''' Error: @Error, si hubo algun error por parte de la instruccion, el @Error indica el error que sucedio
        ///     ''' </summary>
        ///     ''' <param name="Tabla"></param>
        ///     ''' <param name="Columna"></param>
        ///     ''' <param name="Dato"></param>
        ///     ''' <returns> Existe, No Existe, Error: @Error </returns>
        public string ExisteLaClave(string Tabla, string Columna, string Dato)
        {
            string ConsultaClave = "Select * From @Tabla where @Columna='@Dato'";
            ConsultaClave = ConsultaClave.Replace("@Tabla", Tabla).Replace("@Columna", Columna).Replace("@Dato", Dato);
            string ResultadoDeLaConsulta = "No existe";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaClave, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        ResultadoDeLaConsulta = "Existe";
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ResultadoDeLaConsulta = "Error : " + ex.Message;
            }
            return ResultadoDeLaConsulta;
        }
        /// <summary>
        ///     ''' Retorna un string con 'Existe' o 'No existe', o en caso de error, devuelve del error
        ///     ''' </summary>
        ///     ''' <param name="Tabla"></param>
        ///     ''' <param name="Columna1"></param>
        ///     ''' <param name="Dato1"></param>
        ///     ''' <param name="Columna2"></param>
        ///     ''' <param name="Dato2"></param>
        ///     ''' <returns></returns>
        public string ExistsTwoColumnKey(string Tabla, string Columna1, string Dato1, string Columna2, string Dato2)
        {
            string ConsultaClave = "Select TOP 1 PERCENT @Col1 From @Tabla where @Col1='@Dato1' and @Colum2='@Dato2'";
            ConsultaClave = ConsultaClave.Replace("@Tabla", Tabla).Replace("@Col1", Columna1).Replace("@Dato1", Dato1);
            ConsultaClave = ConsultaClave.Replace("@Colum2", Columna2).Replace("@Dato2", Dato2);
            string ResultadoDeLaConsulta = "No existe";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaClave, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        ResultadoDeLaConsulta = "Existe";
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ResultadoDeLaConsulta = "Error : " + ex.Message;
            }
            return ResultadoDeLaConsulta;
        }

        /// <summary>
        ///     ''' Retorna un string con Existe o No existe, o en caso de error, devuelve del error
        ///     ''' "Select TOP 1 PERCENT @Columna From @Tabla where @Columna='@Dato' and @Colum2='@Dato2' and @ExtraCondition"
        ///     ''' </summary>
        ///     ''' <param name="Tabla"></param>
        ///     ''' <param name="Columna1"></param>
        ///     ''' <param name="Dato1"></param>
        ///     ''' <param name="Columna2"></param>
        ///     ''' <param name="Dato2"></param>
        ///     ''' <param name="ExtraCondition"></param>
        ///     ''' <returns></returns>
        public string ExistsTwoColumnKey(string Tabla, string Columna1, string Dato1, string Columna2, string Dato2, string ExtraCondition)
        {
            string ConsultaClave = "Select TOP 1 PERCENT @Columna From @Tabla where @Columna='@Dato1' and @Colum2='@Dato2' and @ExtraCondition";
            ConsultaClave = ConsultaClave.Replace("@Tabla", Tabla).Replace("@Columna", Columna1).Replace("@Dato1", Dato1);
            ConsultaClave = ConsultaClave.Replace("@Colum2", Columna2).Replace("@Dato2", Dato2);
            ConsultaClave = ConsultaClave.Replace("@ExtraCondition", ExtraCondition);
            string ResultadoDeLaConsulta = "No existe";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaClave, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                        ResultadoDeLaConsulta = "Existe";
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ResultadoDeLaConsulta = "Error : " + ex.Message;
            }
            return ResultadoDeLaConsulta;
        }
        /// <summary>
        ///     ''' Se regresa la informacion de la consulta en una lista con el orden previsto según el orden de la otras dos listas
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <param name="MyColumns"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReader2List(string Consulta, List<string> MyColumns)
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        foreach (string str in MyColumns)
                        {
                            if (dr[str] == DBNull.Value)
                                listaC.Add("");
                            else
                                // listaC.Add(dr(str).ToString)
                                listaC.Add(Convert.ToString(dr[str]));
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return listaC;
        }

        // Function SqlReader2ListAllRow(ByVal Consulta As String, ByVal MyErrors As List(Of String))
        // Dim listaC As List(Of String) = New List(Of String)

        // Try
        // Using con As New SqlConnection(ConnectionString)
        // con.Open()
        // Dim cmd As New SqlCommand(Consulta, con)
        // Dim dr As SqlDataReader = cmd.ExecuteReader()

        // End If
        // con.Close()
        // End Using
        // Catch ex As System.Exception
        // MsgBox("Se ha detectado un error:  " + vbCrLf + ex.Message + vbCrLf + Consulta, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas")
        // End Try
        // Return listaC
        // End Function
        /// <summary>
        ///     ''' Se regresa la informacion de la consulta en una lista con el orden previsto según el orden de la otras dos listas
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <param name="MyColumns"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReader2List(string Consulta, List<string> MyColumns, ref List<string> MyErrors)
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        foreach (string str in MyColumns)
                        {
                            if (dr[str] == DBNull.Value)
                                listaC.Add("");
                            else
                                // listaC.Add(dr(str).ToString)
                                listaC.Add(Convert.ToString(dr[str]));
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string FormaError = "" + "Se ha detectado el error: @Error:" +  "\n" + "Debido a la consulta: @Consulta";
                FormaError = FormaError.Replace("@Error", ex.Message.ToString()).Replace("@Consulta", Consulta);
                MyErrors.Add(FormaError);
            }
            return listaC;
        }
        /// <summary>
        ///     ''' Nueva Version de esta funcion, tabla
        ///     ''' "Select " + ConcatenaColumnas(MyColumsInSearch) + " from " + Table + " where " + Condition
        ///     ''' Only Obtain the fist value
        ///     ''' </summary>
        ///     ''' <param name="Table"></param>
        ///     ''' <param name="Condition"></param>
        ///     ''' <param name="MyColumsInSearch"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReader2List(string Table, string Condition, List<string> MyColumsInSearch)
        {
            List<string> listaC = new List<string>();
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            string Consulta = "Select " + ConcatenaColumnas(MyColumsInSearch) + " from " + Table + " where " + Condition;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        foreach (string str in MyColumsInSearch)
                        {
                            if (dr[str] == DBNull.Value)
                                listaC.Add("");
                            else
                                // listaC.Add(dr(str).ToString)
                                listaC.Add(Convert.ToString(dr[str]));
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return listaC;
        }

        public string SqlReader2OneString(string Table, string Condition, string Column)
        {
            string listaC = "";
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            string Consulta = "Select " + Column + " from " + Table + " where " + Condition;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        if (dr[Column] == DBNull.Value)
                            listaC = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            listaC = Convert.ToString(dr[Column]);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return listaC;
        }
        public string SqlReader2OneString(string Consulta, string Columna)
        {
            string listaC = "";
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        if (dr[Columna] == DBNull.Value)
                            listaC = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            listaC = Convert.ToString(dr[Columna]);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return listaC;
        }

        public string SqlReader2OneStringV2(string Table, string Condition, string Column, ref bool IndicadorDeError)
        {
            string StringConsultado = "";
            IndicadorDeError = false;
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            string Consulta = "Select " + Column + " from " + Table + " where " + Condition;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        if (dr[Column] == DBNull.Value)
                            StringConsultado = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            StringConsultado = Convert.ToString(dr[Column]);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                IndicadorDeError = true;
            }
            return StringConsultado;
        }
        public string SqlReader2OneStringV2(string Table, string Condition, string Column, ref List<string> ListaDeErrores)
        {
            string StringConsultado = "";
            // IndicadorDeError = False
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            string Consulta = "Select " + Column + " from " + Table + " where " + Condition;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        if (dr[Column] == DBNull.Value)
                            StringConsultado = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            StringConsultado = Convert.ToString(dr[Column]);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ListaDeErrores.Add(ex.Message.ToString());
            }
            return StringConsultado;
        }
        public List<string> SqlReaderDownOnlyOneColumn(string Table, string Column, ref bool IndicadorDeError, string Condition = "", string ClauseOrder = "")
        {
            List<string> StringConsultado = new List<string>();
            IndicadorDeError = false;
            string Consulta;
            if (Condition.Length == 0)
                Consulta = "Select " + Column + " from " + Table;
            else
                Consulta = "Select " + Column + " from " + Table + " where " + Condition;
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            if (ClauseOrder.Length > 0)
                Consulta = Consulta + " Order By " + ClauseOrder;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[Column] == DBNull.Value)
                            StringConsultado.Add("");
                        else
                            // listaC.Add(dr(str).ToString)
                            StringConsultado.Add(Convert.ToString(dr[Column]));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                IndicadorDeError = true;
            }
            return StringConsultado;
        }
        /// <summary>
        ///     ''' 
        ///     ''' </summary>
        ///     ''' <param name="Table">Tabla a consultar</param>
        ///     ''' <param name="Column">Columna a leer</param>
        ///     ''' <param name="MyListaDeErrores">Son los errores que se generan</param>
        ///     ''' <param name="Condition">Son las clausulas de condiciones</param>
        ///     ''' <param name="ClauseOrder">Aplica la clausula de order</param>
        ///     ''' <param name="IsUnique">Aplica para el primer elemento de la lista</param>
        ///     ''' <returns></returns>
        public List<string> SqlReaderDownOnlyOneColumn(string Table, string Column, ref List<string> MyListaDeErrores, string Condition = "", string ClauseOrder = "", bool IsUnique = false)
        {
            List<string> StringConsultado = new List<string>();
            string Consulta;
            if (IsUnique)
                Consulta = "Select " + Column + " Distinct ";
            else
                Consulta = "Select " + Column;
            if (Condition.Length == 0)
                Consulta = Consulta + " From " + Table;
            else
                Consulta = Consulta + " From " + Table + " where " + Condition;
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            if (ClauseOrder.Length > 0)
                Consulta = Consulta + " Order By " + ClauseOrder;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[Column] == DBNull.Value)
                            StringConsultado.Add("");
                        else
                            // listaC.Add(dr(str).ToString)
                            StringConsultado.Add(Convert.ToString(dr[Column]));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MyListaDeErrores.Add(ex.Message);
            }
            return StringConsultado;
        }
        public List<List<string>> SqlReader2ListOfList(string Table, string Condition, List<string> MyColumsInSearch)
        {
            List<string> ListaC = new List<string>();
            List<List<string>> ListaR = new List<List<string>>();
            // Dim MyColums As List(Of String) = ObtenerColumnasDeTabla(Table)
            string Consulta = "Select " + ConcatenaColumnas(MyColumsInSearch) + " from " + Table + " where " + Condition;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        ListaR.Add(new List<string>());
                        List<string> MyActualList = ListaR[ListaR.Count - 1];
                        foreach (string str in MyColumsInSearch)
                        {
                            if (dr[str] == DBNull.Value)
                                MyActualList.Add("");
                            else
                                // listaC.Add(dr(str).ToString)
                                MyActualList.Add(Convert.ToString(dr[str]));
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return ListaR;
        }

        /// <summary>
        ///     ''' Esta función hace uso de
        ///     ''' Insert Inot TablaSQL Values(MyValues1,MyValues2,...MyValuesN)
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="MyValues"></param>
        ///     ''' <returns></returns>
        public bool InsertaEnSql(string TablaSQL, List<string> MyValues)
        {
            var MyError = false;
            try
            {
                string Comando = "Insert Into " + TablaSQL + " Values (";
                var Limite = MyValues.Count - 1;
                for (int Index = 0; Index <= Limite; Index++)
                {
                    if (Index == Limite)
                        Comando = Comando + MyValues[Index] + ")";
                    else
                        Comando = Comando + MyValues[Index] + ", ";
                }
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Comando, con);
                    var Respuesta = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                //Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Error en el modúlo InsertaEnSQL");
                MyError = true;
            }
            return MyError;
        }

        public List<string> InsertaEnSqlV2(string TablaSQL, List<string> MyValues)
        {
            List<string> MyError = new List<string>();
            try
            {
                string Comando = "Insert Into " + TablaSQL + " Values (";
                var Limite = MyValues.Count - 1;
                for (int Index = 0; Index <= Limite; Index++)
                {
                    if (Index == Limite)
                        Comando = Comando + MyValues[Index] + ")";
                    else
                        Comando = Comando + MyValues[Index] + ", ";
                }
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Comando, con);
                    var Respuesta = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MyError.Add("->InsertaEnSqlV2");
                MyError.Add(ex.Message);
            }
            return MyError;
        }

        public List<string> InsertaSQLSpecificColums(string TablaSQL, List<string> MyValues, List<string> ToColumns)
        {
            List<string> MyError = new List<string>();
            if (MyValues.Count == ToColumns.Count)
            {
                try
                {
                    string Comando = "Insert Into " + TablaSQL + " (";
                    int MyColumnsCount = ToColumns.Count - 1;
                    for (int Index = 0; Index <= MyColumnsCount; Index++)
                    {
                        if (Index == MyColumnsCount)
                            Comando = Comando + ToColumns[Index] + ")";
                        else
                            Comando = Comando + ToColumns[Index] + ", ";
                    }
                    Comando = Comando + " Values (";
                    var Limite = MyValues.Count - 1;
                    for (int Index = 0; Index <= Limite; Index++)
                    {
                        if (Index == Limite)
                            Comando = Comando + MyValues[Index] + ")";
                        else
                            Comando = Comando + MyValues[Index] + ", ";
                    }
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(Comando, con);
                        int Respuesta = cmd.ExecuteNonQuery();
                        if (Respuesta == 0)
                            MyError.Add("No se ha efectuar el registro en la base de datos");
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    MyError.Add("->InsertaEnSqlV2");
                    MyError.Add(ex.Message);
                }
            }
            else
                MyError.Add("Las listas no son del mismo tamaño, nota del programador, check InsertaSQLSpecificColums");
            return MyError;
        }
        /// <summary>
        ///     ''' Los valores como @NULL, pueden ser remplazados como NULL, sin comillas para eliminar los valores que no tienen validad
        ///     ''' Dentro del comando '@NULL'->NULL
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="Data2Insert"></param>
        ///     ''' <returns></returns>
        public string InsertaSQLSpecificColums(string TablaSQL, SortedList<string, string> Data2Insert)
        {
            string ErrorSql = "";
            string ComandoSql = "Insert Into @Table (@Columnas) Values (@Data)";
            var ConcatenacionColumnas = "";
            var MaxIndexRegister = Data2Insert.Keys.Count - 1;
            var ConcatenacionData = "";
            for (int IndexRegister = 0; IndexRegister <= MaxIndexRegister; IndexRegister++)
            {
                if (IndexRegister == MaxIndexRegister)
                {
                    ConcatenacionColumnas += Data2Insert.Keys[IndexRegister];
                    ConcatenacionData += "'" + Data2Insert.Values[IndexRegister] + "'";
                }
                else
                {
                    ConcatenacionColumnas += Data2Insert.Keys[IndexRegister] + ", ";
                    ConcatenacionData += " '" + Data2Insert.Values[IndexRegister] + "'" + ",";
                }
            }
            ComandoSql = ComandoSql.Replace("@Table", TablaSQL);
            ComandoSql = ComandoSql.Replace("@Columnas", ConcatenacionColumnas);
            ComandoSql = ComandoSql.Replace("@Data", ConcatenacionData);
            ComandoSql = ComandoSql.Replace("'@ServerDT'", "SYSDATETIME()");
            ComandoSql = ComandoSql.Replace("'@NULL'", "NULL");

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ComandoSql, con);
                    int Respuesta = cmd.ExecuteNonQuery();
                    if (Respuesta == 0)
                        ErrorSql = "No se ha logrado registrar la informacion en la base de datos: " + ComandoSql;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorSql = ex.Message +  "\n" + "ComandoSql: " + ComandoSql;
            }
            return ErrorSql;
        }
        /// <summary>
        ///     ''' With Column Of Hour, The data that you put in ToReplaceWithHourServer is replaced by  SYSDATETIME()
        ///     ''' </summary>
        ///     ''' <param name="TablaSQL"></param>
        ///     ''' <param name="Data2Insert"></param>
        ///     ''' <returns></returns>
        public string InsertaSQLSpecificColums(string TablaSQL, SortedList<string, string> Data2Insert, string ToReplaceWithHourServer)
        {
            string ErrorSql = "";
            string ComandoSql = "Insert Into @Table (@Columnas) Values (@Data)";
            var ConcatenacionColumnas = "";
            var MaxIndexRegister = Data2Insert.Keys.Count - 1;
            var ConcatenacionData = "";
            for (int IndexRegister = 0; IndexRegister <= MaxIndexRegister; IndexRegister++)
            {
                if (IndexRegister == MaxIndexRegister)
                {
                    ConcatenacionColumnas += Data2Insert.Keys[IndexRegister];
                    ConcatenacionData += "'" + Data2Insert.Values[IndexRegister] + "'";
                }
                else
                {
                    ConcatenacionColumnas += Data2Insert.Keys[IndexRegister] + ", ";
                    ConcatenacionData += " '" + Data2Insert.Values[IndexRegister] + "'" + ",";
                }
            }
            ComandoSql = ComandoSql.Replace("@Table", TablaSQL);
            ComandoSql = ComandoSql.Replace("@Columnas", ConcatenacionColumnas);
            ComandoSql = ComandoSql.Replace("@Data", ConcatenacionData);
            ComandoSql = ComandoSql.Replace("'" + ToReplaceWithHourServer + "'", "SYSDATETIME()");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ComandoSql, con);
                    int Respuesta = cmd.ExecuteNonQuery();
                    if (Respuesta == 0)
                        ErrorSql = "No se ha logrado registrar la informacion en la base de datos: " + ComandoSql;
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorSql = ex.Message +  "\n" + "ComandoSql: " + ComandoSql;
            }
            return ErrorSql;
        }
        public SortedList<string, string> GetRegistersOfClv(string TablaSql, string Columna, string Clave)
        {
            List<string> ListaDeColumnas = new List<string>();
            SortedList<string, string> DataRegisters = new SortedList<string, string>();
            ListaDeColumnas = ObtenerColumnasDeTabla(TablaSql);
            string Consulta = "Select " + ConcatenaColumnas(ListaDeColumnas) + " from " + TablaSql + " where " + Columna + "='" + Clave + "';";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if ((dr.Read()))
                    {
                        foreach (string str in ListaDeColumnas)
                        {
                            if (dr[str] == DBNull.Value)
                                DataRegisters.Add(str.Trim(), " ");
                            else
                                // listaC.Add(dr(str).ToString)
                                DataRegisters.Add(str.Trim(), Convert.ToString(dr[str]).Trim());
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", Consulta);
                //Interaction.MsgBox(TemporalConcat, MsgBoxStyle.Critical, "Error en el modúlo ConsultaDeLinea Consulta-Columnas");
            }
            return DataRegisters;
        }
        /// <summary>
        ///     ''' Permite pasar de
        ///     ''' Dato-> 'Dato'
        ///     ''' Pero con elementos de una lista
        ///     ''' </summary>
        ///     ''' <param name="MyList"></param>
        ///     ''' <returns></returns>
        public List<string> InsertComillas(List<string> MyList)
        {
            List<string> Retorno = new List<string>();
            foreach (string str in MyList)
                Retorno.Add("'" + str + "'");
            return Retorno;
        }

        public string InsertComillas(string MyDato)
        {
            return " '" + MyDato + "' ";
        }

        public List<string> InsertComillas(List<string> MyList, int Pini, int Pfin)
        {
            List<string> Retorno = new List<string>();
            try
            {
                for (int Index = 0; Index <= MyList.Count - 1; Index++)
                {
                    if ((Pini <= Index & Index <= Pfin))
                        Retorno.Add(" '" + MyList[Index] + "' ");
                    else
                        Retorno.Add(MyList[Index]);
                }
            }
            catch (Exception er)
            {
                //Interaction.MsgBox(er.Message);
            }
            return Retorno;
        }

        /// <summary>
        ///     ''' Permite ejecutar la consulta definida, siendo la TablaSQL unicamente como dato informativo.
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReaderDown2List(string Consulta, ref string ErrorOut )
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[0] != DBNull.Value && dr[0] != null) {
                            listaC.Add(Convert.ToString(dr[0]));
                        }
                        else
                        {
                            listaC.Add("");
                        }

                    }
                    // If listaC.Count = 0 Then
                    // listaC.Add("V")
                    // End IfBu
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                listaC.Clear();
                ErrorOut = ex.Message;
            }
            return listaC;
        }
        /// <summary>
        ///     ''' 
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReaderDown2List(string Consulta, ref List<string> ListaDeErrores)
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[0] != DBNull.Value && dr[0] != null)
                        {
                            listaC.Add(Convert.ToString(dr[0]));
                        }
                        else
                        {
                            listaC.Add("");
                        }
                    }
                    // If listaC.Count = 0 Then
                    // listaC.Add("V")
                    // End If
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                listaC.Clear();
                string TemporalConcat = "Se ha presentado el error: " + ex.Message +  "\n" + "al ejecutar la consulta: " + Consulta;
                ListaDeErrores.Add(TemporalConcat);
            }
            return listaC;
        }

        /// <summary>
        ///     ''' Permite ejecutar la consulta definida, siendo la TablaSQL unicamente como dato informativo.
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReaderDown2List(string Consulta, ref bool IndicadorErr)
        {
            IndicadorErr = false;
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[0] != DBNull.Value && dr[0] != null)
                        {
                            listaC.Add(Convert.ToString(dr[0]));
                        }
                        else
                        {
                            listaC.Add("");
                        }
                    }
                    // If listaC.Count = 0 Then
                    // listaC.Add("V")
                    // End If
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                //Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Error en el modúlo SqlReaderDown2List");
                IndicadorErr = true;
            }
            return listaC;
        }
        /// <summary>
        ///     ''' Permite ejecutar la consulta definida, siendo la TablaSQL unicamente como dato informativo.
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <returns></returns>
        public List<string> SqlReaderDown2ListWithDetailError(string Consulta, ref List<string> IndicadorErr)
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[0] != DBNull.Value && dr[0] != null)
                        {
                            listaC.Add(Convert.ToString(dr[0]));
                        }
                        else
                        {
                            listaC.Add("");
                        }
                    }
                    // If listaC.Count = 0 Then
                    // listaC.Add("V")
                    // End If
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // 'MsgBox(ex.Message, MsgBoxStyle.Critical, "Error en el modúlo SqlReaderDown2List")
                IndicadorErr.Add(ex.Message);
            }
            return listaC;
        }
        /// <summary>
        ///     ''' Permite ejecutar la consulta definida, siendo la TablaSQL unicamente como dato informativo.
        ///     ''' </summary>
        ///     ''' <param name="Consulta"></param>
        ///     ''' <returns></returns>
        public async Task<List<string>> SqlReaderDown2ListAsync(string Consulta)
        {
            List<string> listaC = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Consulta, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        if (dr[0] != DBNull.Value && dr[0] != null)
                        {
                            listaC.Add(Convert.ToString(dr[0]));
                        }
                        else
                        {
                            listaC.Add("");
                        }
                    }
                    // If listaC.Count = 0 Then
                    // listaC.Add("V")
                    // End If
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // 'MsgBox(ex.Message, MsgBoxStyle.Critical, "Error en el modúlo SqlReaderDown2List")
                listaC.Clear();
                listaC.Add(ex.Message);
            }
            return listaC;
        }
        /// <summary>
        ///     ''' Permite realizar la consulta de esta manera
        ///     ''' Select MyColR from MyTable where MyColB='MyStr'
        ///     ''' </summary>
        ///     ''' <param name="Mytable"></param>
        ///     ''' <param name="MyColR"></param>
        ///     ''' <param name="MyColB"></param>
        ///     ''' <param name="MyStr"></param>
        ///     ''' <returns></returns>
        public string BusquedaDeCambioEnSql(string Mytable, string MyColR, string MyColB, string MyStr, ref string OutputError )
        {
            bool MyError = true;
            string Consulta = "Select " + MyColR + " from " + Mytable + " where " + MyColB + " ='" + MyStr + "'";
            OutputError = "";
            List<string> MyR = SqlReaderDown2List(Consulta, ref OutputError);
            if (MyR.Count == 0)
                return ""; // e de error
            else
                return MyR[0];
        }

        /// <summary>
        ///     ''' Aquí se cargan los registros que consideramos valiosos, recuerda encomillar aquellos datos que de verdad
        ///     ''' lo necesiten
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public bool  InsertaEnSql(string MyTable, List<string> MyColumns, List<string> MyData)
        {
            var MyError = false;
            try
            {
                if (MyColumns.Count == MyData.Count)
                {
                    string Comando = "Insert Into " + MyTable + "(";
                    int Index = 0;
                    int UltIn = MyColumns.Count - 1;
                    for (Index = 0; Index <= UltIn; Index++)
                    {
                        if (Index == UltIn)
                            Comando = Comando + MyColumns[Index] + ")";
                        else
                            Comando = Comando + MyColumns[Index] + ", ";
                    }
                    Comando = Comando + " values(";
                    for (Index = 0; Index <= UltIn; Index++)
                    {
                        if (Index == UltIn)
                            Comando = Comando + MyTable[Index] + ")";
                        else
                            Comando = Comando + MyTable[Index] + ", ";
                    }
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(Comando, con);
                        var Respuesta = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                    //Interaction.MsgBox("Las listas no tienen el mismo tamaño en InsertaEnSql", Title: "AdminSQL");
            }
            catch (Exception er)
            {
                //Interaction.MsgBox(er.Message, Title: "AdminSQL");
            }
            return MyError;
        }

        /// <summary>
        ///     ''' Return -1 en caso de un error con SQL
        ///     ''' Return 0 si no esta vacia
        ///     ''' Return 1 si esta vacia
        ///     ''' </summary>
        ///     ''' <param name="MyTable"></param>
        ///     ''' <returns></returns>
        public int IsEmplyTheTable(string MyTable)
        {
            bool IndicaError = false;
            List<string> MyColumns = ObtenerColumnasDeTabla(MyTable);
            if (MyColumns.Count > 0)
            {
                List<string> Data = SqlReaderDown2List(ArmaConSQLColumnas(MyTable, MyColumns[0], 1), ref IndicaError);
                if (IndicaError)
                    return -1;
                if (Data.Count > 0)
                    return 0;
                else
                    return 1;
            }
            else
                return -2;
        }

        public string ArmaConSQLColumnas(string MyTable, string Columna)
        {
            return "Select " + Columna + " From " + MyTable;
        }

        public string ArmaConSQLColumnas(string MyTable, List<string> Columnas)
        {
            string MyConsulta = "Select ";
            if (Columnas.Count > 0)
            {
                var MaximoI = Columnas.Count - 1;
                for (int Index = 0; Index <= MaximoI; Index++)
                {
                    if (Index == MaximoI)
                        MyConsulta = MyConsulta + Columnas[Index];
                    else
                        MyConsulta = MyConsulta + Columnas[Index] + ", ";
                }
                MyConsulta = MyConsulta + " from " + MyTable;
            }
            return MyConsulta;
        }

        public string ArmaConSQLColumnas(string MyTable, string Columna, int LimitAt)
        {
            return "Select Top " + LimitAt.ToString() + Columna + " From " + MyTable;
        }

        public List<string> ObtenerColumnasDeTabla(string MyTable)
        {
            var ConsultaClv = "Select Column_name from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + MyTable + "'";
            var _Error = "";
            List<string> MyExistence = SqlReaderDown2List(ConsultaClv, ref  _Error);
            return MyExistence;
        }
        /// <summary>
        ///     ''' Recuerda que este método conserva todos los metodos y clases derivadas de cada uno de los campos, es decir
        ///     ''' Que si en la base de datos es una fecha, aquí tambien es una fecha, lo que no he determinado es si mantiene las relaciones
        ///     ''' de correspondencia, es decir, si se trata de una Foregin Key si conserva esa relacion directamente, por que si fuera así nos
        ///     ''' ahorraria mucho trabajo de aquí en delante.
        ///     ''' </summary>
        ///     ''' <param name="MyConsulta"></param>
        ///     ''' <returns></returns>
        public DataTable DataParaTable(string MyConsulta, ref string StringErrorOut )
        {
            DataTable MyTable = new DataTable();
            SqlDataAdapter MyData;
            MyConsulta = MyConsulta.Trim();
            try
            {
                using (SqlConnection MyCon = new SqlConnection(RetornaElConnectionString()))
                {
                    MyCon.Open();
                    MyData = new SqlDataAdapter(MyConsulta, MyCon);
                    MyData.Fill(MyTable);
                    MyCon.Close();
                }
                return MyTable;
            }
            catch (Exception ex)
            {
                StringErrorOut = ex.Message.ToString();
                return MyTable;
            }
        }

        /// <summary>
        ///     ''' Esta funcion retorna un uno en caso de exito, -3 en caso de que ya exista
        ///     ''' -2 En caso de que exista una tabla con el número de columnas incompletas
        ///     ''' -1 En el caso de un error en SQL y no se haya creado
        ///     ''' </summary>
        ///     ''' <param name="NombreTb"></param>
        ///     ''' <param name="Columnas"></param>
        ///     ''' <returns></returns>
        public int CreaTb(string NombreTb, List<string> Columnas)
        {
            // Inicial setting of variables'
            var Existe = IsTableExists(NombreTb, Columnas.Count);
            if (Existe == -1)
            {
                var MyStartStr = "Create table " + NombreTb + " (";
                int MaxiIndex = Columnas.Count - 1;
                for (int Index = 0; Index <= MaxiIndex; Index++)
                {
                    if (Index != MaxiIndex)
                        MyStartStr = MyStartStr + Columnas[Index] + ", ";
                    else
                        MyStartStr = MyStartStr + Columnas[Index] + ") ";
                }
                var Comando = MyStartStr;
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(Comando, con);
                    var Respuesta = cmd.ExecuteNonQuery();
                    if (Respuesta > 0)
                        return 1;
                    else
                        return 0;
                    con.Close();
                }
                return -1;
            }
            else if (Existe == 0)
                return -2;
            else
                return -3;
        }

        /// <summary>
        ///     ''' Permite conocer si una tabla en Sql existe de manera contraria, regresa un -1
        ///     ''' El caso 0 es si no contiene el número esperado de columnas la tabla
        ///     ''' </summary>
        ///     ''' <param name="NombreTb"></param>
        ///     ''' <param name="NumberOfColums"></param>
        ///     ''' <returns></returns>
        public int IsTableExists(string NombreTb, int NumberOfColums)
        {
            var BusquedaDeTablaExistente = "Select Column_name From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME = '" + NombreTb + "'";
            var _Error = "";
            List<string> MyExistence = SqlReaderDown2List(BusquedaDeTablaExistente, ref  _Error);
            if (MyExistence.Count == NumberOfColums)
                return 1;
            else if (MyExistence.Count > 0)
                return 0;
            else
                return -1;
        }

        public string ConcatenaColumnas(List<string> MyColumnas)
        {
            string Concatenacion = "";
            foreach (var _Str in MyColumnas)
                Concatenacion = Concatenacion + " " + _Str + ",";
            return Concatenacion.Remove(Concatenacion.Length - 1);
        }

        public void DeleteAnRegister(string Table, List<string> ListOfConditiones, ref List<string> ListaDeErrores)
        {
            string Comando = "Delete From @Table where @Conditions";
            if (ListOfConditiones.Count > 0)
            {
                string Condicional = JoinTheConditionsClauses(ListOfConditiones);
                Comando = Comando.Replace("@Table", Table);
                Comando = Comando.Replace("@Conditions", Condicional);
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(Comando, con);
                        var Respuesta = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    ListaDeErrores.Add(ex.Message);
                }
            }
            else
                ListaDeErrores.Add("No se han indicado los parametros para eliminar un registro");
        }
        public string DeleteAnRegister(string Table, List<string> ListOfConditiones)
        {
            string Comando = "Delete From @Table where @Conditions";
            if (ListOfConditiones.Count > 0)
            {
                string Condicional = JoinTheConditionsClauses(ListOfConditiones);
                Comando = Comando.Replace("@Table", Table);
                Comando = Comando.Replace("@Conditions", Condicional);
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(Comando, con);
                        var Respuesta = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
                return "No se han indicado los parametros para eliminar un registro";
            return "";
        }

        public List<string> ExecuteOnSqlImportingTheNumberOfAffectedRows(string SqlComand)
        {
            List<string> ListaDeErrores = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(SqlComand, con);
                    var respuesta = cmd.ExecuteNonQuery();
                    if (respuesta == 0)
                        // Exito = False
                        ListaDeErrores.Add("No se afecto ninguna fila:\n"  + SqlComand);
                    con.Close();
                }
            }
            catch (Exception er)
            {
                ListaDeErrores.Add(er.Message);
            }
            return ListaDeErrores;
        }
        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBoxWithOutDescription(string TableSql, string ColumnValue, string ColumnStatus, ref string ErrorSql, string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select DISTINCT @Values, @Status from @Table @WhereClause Order by @Values;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Status", ColumnStatus);
            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar = "";
                        string AStatus;
                        string AValor;
                        if (dr[ColumnStatus] == DBNull.Value)
                            AStatus = "";
                        else
                            AStatus = Convert.ToString(dr[ColumnStatus]);
                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);
                        NewListOfkyes.Add(new KeyValuePair<string, string>(AValor + ": \n" + AStatus, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }
        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBox(string TableSql, string ColumnaDisplayMember, string ColumnStatus, string ColumnValue, ref string ErrorSql, string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select DISTINCT @Values, @Display, @Status from @Table @WhereClause Order by @Display;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Display", ColumnaDisplayMember);
            ConsultaSql = ConsultaSql.Replace("@Status", ColumnStatus);
            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar;
                        string AStatus;
                        string AValor;
                        if (dr[ColumnaDisplayMember] == DBNull.Value)
                            AMostrar = "";
                        else
                            AMostrar = Convert.ToString(dr[ColumnaDisplayMember]);
                        if (dr[ColumnStatus] == DBNull.Value)
                            AStatus = "";
                        else
                            AStatus = Convert.ToString(dr[ColumnStatus]);
                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);
                        NewListOfkyes.Add(new KeyValuePair<string, string>(AMostrar + ": \n"+ AStatus, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }

        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBoxCustomDescription(string TableSql, string ExpresionDescription, string AliasDescription, string ColumnValue, ref string ErrorSql, string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select @Values, @Expresion from @Table @WhereClause Order by @Alias;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Alias", AliasDescription);
            if (ExpresionDescription.Contains(" as "))
                ConsultaSql = ConsultaSql.Replace("@Expresion", ExpresionDescription);
            else
                ConsultaSql = ConsultaSql.Replace("@Expresion", ExpresionDescription + " as [" + AliasDescription + "]");
            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar;
                        string AStatus;
                        string AValor;
                        if (dr[AliasDescription] == DBNull.Value)
                            AMostrar = "";
                        else
                            AMostrar = Convert.ToString(dr[AliasDescription]);

                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);

                        NewListOfkyes.Add(new KeyValuePair<string, string>(AMostrar, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }

        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBoxValueDescriptionPlusState(string TableSql, string Description, string StatusExpresion, string StatusAlias, string ColumnValue, ref string ErrorSql, string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select @Values, @Description, @Expresion from @Table @WhereClause Order by @Description;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Description", Description);
            if (StatusExpresion.Contains(" as "))
                ConsultaSql = ConsultaSql.Replace("@Expresion", StatusExpresion);
            else
                ConsultaSql = ConsultaSql.Replace("@Expresion", StatusExpresion + " as [" + StatusAlias + "]");
            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar;
                        string AStatus;
                        string ADescription;
                        string AValor;
                        if (dr[StatusAlias] == DBNull.Value)
                            AStatus = "";
                        else
                            AStatus = Convert.ToString(dr[StatusAlias]);
                        if (dr[Description] == DBNull.Value)
                            ADescription = "";
                        else
                            ADescription = Convert.ToString(dr[Description]);

                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);
                        AMostrar = ADescription + ": " + AStatus;
                        NewListOfkyes.Add(new KeyValuePair<string, string>(AMostrar, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }

        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBoxWithoutOrder(string TableSql, string ColumnaDisplayMember, string ColumnStatus, string ColumnValue, ref string ErrorSql, string DefineOrder = "", string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select DISTINCT @Values, @Display, @Status from @Table @WhereClause @OrderClause;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Display", ColumnaDisplayMember);
            ConsultaSql = ConsultaSql.Replace("@Status", ColumnStatus);
            if (DefineOrder.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@OrderClause;", "Order by " + DefineOrder);
            else
                ConsultaSql = ConsultaSql.Replace("@OrderClause;", ";");
            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar;
                        string AStatus;
                        string AValor;
                        if (dr[ColumnaDisplayMember] == DBNull.Value)
                            AMostrar = "";
                        else
                            AMostrar = Convert.ToString(dr[ColumnaDisplayMember]);
                        if (dr[ColumnStatus] == DBNull.Value)
                            AStatus = "";
                        else
                            AStatus = Convert.ToString(dr[ColumnStatus]);
                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);
                        NewListOfkyes.Add(new KeyValuePair<string, string>(AMostrar + ": \n" + AStatus, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }

        public List<KeyValuePair<string, string>> GetkeyPairs2ComboBoxWithOutStatus(string TableSql, string ColumnaDisplayMember, string ColumnValue, ref string ErrorSql, string Condition = "")
        {
            List<KeyValuePair<string, string>> NewListOfkyes = new List<KeyValuePair<string, string>>();
            string ConsultaSql = "Select DISTINCT @Values, @Display from @Table @WhereClause Order by @Display;";
            ConsultaSql = ConsultaSql.Replace("@Table", TableSql);
            ConsultaSql = ConsultaSql.Replace("@Values", ColumnValue);
            ConsultaSql = ConsultaSql.Replace("@Display", ColumnaDisplayMember);

            if (Condition.Length > 0)
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "Where " + Condition);
            else
                ConsultaSql = ConsultaSql.Replace("@WhereClause", "");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(ConsultaSql, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while ((dr.Read()))
                    {
                        string AMostrar;
                        string AValor;
                        if (dr[ColumnaDisplayMember] == DBNull.Value)
                            AMostrar = "";
                        else
                            AMostrar = Convert.ToString(dr[ColumnaDisplayMember]);

                        if (dr[ColumnValue] == DBNull.Value)
                            AValor = "";
                        else
                            // listaC.Add(dr(str).ToString)
                            AValor = Convert.ToString(dr[ColumnValue]);
                        NewListOfkyes.Add(new KeyValuePair<string, string>(AMostrar, AValor));
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                string TemporalConcat = "Se ha detectado un error: @Error" +  "\n" + "Consulta: @Co1";
                TemporalConcat = TemporalConcat.Replace("@Error", ex.Message.ToString()).Replace("@Co1", ConsultaSql);
                ErrorSql = TemporalConcat;
            }
            return NewListOfkyes;
        }

        //public object ReturnLastChildren(ref object MyStackPanel)
        //{
        //    return MyStackPanel.Children(MyStackPanel.Children.Count - 1);
        //}

    }

}
