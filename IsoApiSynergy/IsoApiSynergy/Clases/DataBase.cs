using System;
using System.Data.SqlClient;
namespace IsoApiSynergy
{
    public class DataBase
    {
        public SqlCommand conexion = new SqlCommand();
        public SqlConnection connection = new SqlConnection();
        public Boolean ErrorConexion = false;
        public DataBase()
        {
            string cadena = "Server=127.0.0.1;Database=ISO;User Id=sa;Password=PassWord123;";
            try
            {
                connection.ConnectionString = cadena;
            }
            catch
            {
                ErrorConexion = true;
            }


            try
            {
                connection.Open();
                conexion.Connection = connection;
            }
            catch
            {
                ErrorConexion = true;
            }

        }


        public bool getConnection()
        {
            Console.WriteLine("estatus {0}", connection.State);
            if (connection.State.ToString().Equals("Open"))
            {
                return true;
            }

            return false;
        }


        public void Dispose()
        {
            conexion.Connection.Close();
            connection.Close();

        }
    }
}
