using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Net;
using System.Web.Services;
using System.Web.UI.WebControls;


/// <summary>
/// Descripción breve de WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{
    [WebMethod]
    public DataTable traerTabla(string nombreTabla)// se crea el metodo con un parametro establecido nombreTabla
    {

        OleDbConnection conexion = estableceConexion();
        DataTable table = new DataTable(nombreTabla);
        try
        {
            string query = "Select * from " + nombreTabla; // creamos la instruccion SQL
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion);
            adapter.Fill(table);
            conexion.Open();
            
            //TraerUltimoConsecutivo(nombreTabla);
            //InsertarFila(table, nombreTabla);
            ActualizarFilas(table);   
            //insertarLog(1,2, obtenerip(), 2);




        }
        catch (Exception ex)
        {
            Console.WriteLine("Error No se pudo traer los datos" + ex.Message);
        }
        return table;


    }

    [WebMethod]
    public DataTable TraerUltimoConsecutivo(string tabla)
    {
        OleDbConnection conexion = estableceConexion();

        DataTable table = new DataTable("id");
        try
        {
            string query = "SELECT  IDENT_CURRENT ('" + tabla + "')";
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion);
            adapter.Fill(table);
            conexion.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return table;


    }

    [WebMethod]
    private string obtenerip()
    {
        string h_n = Dns.GetHostName();
        return h_n;
    }




    [WebMethod]//metodo para insertar logs
    private DataTable insertarLog(int idAccion, int idConsulta, string ipEquipo, int idUsuario)
    {


        OleDbConnection conexion = estableceConexion();
        DataTable table = new DataTable("Logs");
        try
        {
            string query = "Select * from Logs";
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion);
            adapter.FillSchema(table, SchemaType.Source);
            conexion.Open();
            DataRow fila = table.NewRow();
            fila.BeginEdit();
            fila[1] = idAccion;
            fila[2] = idConsulta;
            fila[3] = ipEquipo;
            fila[4] = DateTime.Now; // Convert.ToDateTime(fecha);
            fila[5] = idUsuario;
            fila.EndEdit();
            table.Rows.Add(fila);

            InsertarFila(table, "Logs");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return table;

    }

    [WebMethod]//metodo para traer la fecha y hora actual
    private string fechaActual()
    {
        DateTime localDate = DateTime.Now;
        //string formattedDate = localDate.ToString("MM/dd/yyyy HH:mm:ss");
        //Console.WriteLine(formattedDate + " " + formattedDate); 

        string formattedDate = localDate.ToString("yyyy-dd-MM HH:mm:ss.000");
        return formattedDate;
    }
    [WebMethod] //Metodo para establecer la conexion
    private OleDbConnection estableceConexion()
    {
        return new OleDbConnection(Desencriptacion());
    }


    [WebMethod] //metodo para actualizar fila
    private DataTable ActualizarFilas(DataTable tabla)

    {
       
        OleDbConnection conexion = estableceConexion();
        try
        {
            DataRow valor = tabla.NewRow();  //creamos una variable de tipo DataRow que se encargara de definir una nueva fila en la tabla
            valor.BeginEdit(); //la funcion BeginEdit() abre la edicion a la fila de la tabla
            valor[1] = "El cubo rubik 3x3"; //editamos los campos que requerimos 
            valor[2] = "Algoritmo para resolver el cubo";
            valor[3] = "1";
            valor[4] = "Servidor gep";
            valor[5] = false;
            valor[6] = "dfdsdf";
            valor.EndEdit(); //cerramos la edicion
            tabla.Rows.Add(valor);//agregamos la nueva fila a la tabla 


            foreach (DataRow row in tabla.Rows) {
                if (!string.IsNullOrEmpty(row[0].ToString()) || row[0].ToString()=="0")//evitamos que lea la longitud 0 de la matriz 
                {
                    string instruccionSQL = "UPDATE " + tabla + " SET ";
                    string campos = "";
                    for (int i = 0; i < tabla.Columns.Count; i++)
                    {
                        instruccionSQL += tabla.Columns[i].ColumnName + " = ";
                        switch (tabla.Columns[i].DataType.ToString()) //creamos un switch para recorrer los diferentes tipos de datos de las columnas
                        {
                            case "System.String": //caso que sea String
                                campos += row[i].ToString() + valor +","; //se agrgara  una coma

                                break;
                            case "System.Boolean": //caso que sea Boolean
                                campos += row[i].ToString() == "True" ? "1,": "0,"; //se agrgara 1 o 0

                                break;
                            case "System.DateTime": //caso que sea Fecha
                                string Fecha = Convert.ToDateTime(row[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                campos += "'" + Fecha + "'"; //se agrega la fecha

                                break;
                            default: //por defecto
                                campos += row[i].ToString() + ",";//se agrgara 1 o 0


                                break;
                        }
                    }
                    OleDbCommand query = new OleDbCommand(instruccionSQL, conexion);
                    instruccionSQL = instruccionSQL.Substring(0, instruccionSQL.Length - 1);
                    campos = campos.Substring(0, campos.Length - 1);
                    instruccionSQL = instruccionSQL + campos;


                    conexion.Open();
                    query.ExecuteNonQuery();
                    conexion.Close();
                }
                
            }
           
            
                
          



        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);  
        }
        return tabla;
    }

    [WebMethod]
    private void InsertarFila(DataTable table, string nombreTabla)
    {

        DataRow filanueva = table.NewRow();  //creamos una variable de tipo DataRow que se encargara de definir una nueva fila en la tabla
        filanueva.BeginEdit(); //la funcion BeginEdit() abre la edicion a la fila de la tabla
        filanueva[1] = "El cubo rubik 3x3"; //editamos los campos que requerimos 
        filanueva[2] = "Algoritmo para resolver el cubo";
        filanueva[3] = "1";
        filanueva[4] = "Servidor gep";
        filanueva[5] = false;
        filanueva[6] = "dfdsdf";
        filanueva.EndEdit(); //cerramos la edicion
        table.Rows.Add(filanueva);//agregamos la nueva fila a la tabla 
        
        foreach (DataRow FilaTabla in table.Rows) //creamos un foreach para recorrer las filas de la tabla
        {
            if (string.IsNullOrEmpty(FilaTabla[0].ToString()) || FilaTabla[0].ToString()=="0") //evadimos que que inserte el dato si es nulo en la logitud 0
            {
                string InstruccionSQL = "Insert into " + nombreTabla + "(";  //creamos una variable que contenga una parte de la sentencia SQL
                string Valores = " Values("; //y esta contiene otra parte

                for (int Columna = 1; Columna < table.Columns.Count; Columna++)//creamos un for para recorrer las columnas de la tabla despuesde de la longitud 1
                {
                    InstruccionSQL += table.Columns[Columna].ColumnName + ",";//y hacermos que se incremetnte por cada nombre de la columna una ","

                    switch (table.Columns[Columna].DataType.ToString()) //creamos un switch para recorrer los diferentes tipos de datos de las columnas
                    {
                        case "System.String": //caso que sea String
                            Valores += "'" + FilaTabla[Columna].ToString() + "',"; //se agrgara comillas simples y una coma
                            break;
                        case "System.Boolean": //caso que sea Boolean
                            Valores += FilaTabla[Columna].ToString() == "True" ? "1," : "0,"; //su es true escribira 1 y si es false escribira 0

                            break;
                        case "System.DateTime": //caso que sea Fecha
                            string Fecha = Convert.ToDateTime(FilaTabla[Columna].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            Valores += "'" + Fecha + "',"; //se agrgara comillas simples y una coma
                            
                            break;
                        default: //por defecto
                            Valores += FilaTabla[Columna].ToString() + ","; //solo escribira una ,
                            break;
                    }
                }

                OleDbConnection conexion = estableceConexion();
                conexion.Open();
                InstruccionSQL = InstruccionSQL.Substring(0, InstruccionSQL.Length - 1) + ")"; // aqui le quitamos el ultimo acaracter al string y le colocamos un parentesis cerrado
                Valores = Valores.Substring(0, Valores.Length - 1) + ")"; //aqui le quitamos el ultimo caracter al string y le colocamos un parentesis
                InstruccionSQL = InstruccionSQL + Valores;//y aqui almacenamos la concatenacion de la consulta

                OleDbCommand envio = new OleDbCommand(InstruccionSQL, conexion);//definimos que es lo que se va a "ejecutar"
                envio.ExecuteNonQuery();   //ejecutamos el query
                conexion.Close();

            }
        }
    }

    [WebMethod]//metodo para desencriptar la informacion
    private string Desencriptacion()
    {
        string provider = ConfigurationManager.AppSettings["Provider"];
        string servidor = ConfigurationManager.AppSettings["Servidor"];
        string basededatos = ConfigurationManager.AppSettings["BaseDeDatos"];
        string user = ConfigurationManager.AppSettings["User"];
        string pasw = ConfigurationManager.AppSettings["Pasw"];

        return ConnectionDB(provider, servidor, user, pasw, basededatos);
    }

    [WebMethod] //metodo para establecer la cadena de  conexion
    private string ConnectionDB(string provider, string servidor, string user, string pasw, string basededatos)
    {
        try
        {
            string cadenaConexion = "Provider=" + provider + ";Data Source=" + servidor + ";Persist Security Info=True; User ID=" + user + ";Password = " + pasw + ";Initial Catalog=" + basededatos;
            Console.WriteLine("Conexion exitosa!");
            return cadenaConexion;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
