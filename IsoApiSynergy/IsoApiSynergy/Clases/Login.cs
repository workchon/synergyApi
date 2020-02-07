using System;
using System.Data;
using System.Data.SqlClient;
namespace IsoApiSynergy.Clases
{
    public class Login
    {
        public DataBase con = new DataBase();
        public struct strLogin
        {
            public int idUsuario;
            public string nombreUsuario;
            public string NombreCorto;
            public string passwordUsuario;
            public int NumeroEmpleado;
            public DateTime FechaPassword;
            public string IconoUsuario;
            public bool Estatus;
            public string Email;

            public int idEmpresa;
        }


        public bool verificarUsuario(ref DataSet DS, strLogin datos)
        {
            DS = new DataSet();
            con.conexion.CommandText = "select * from ISOHS001 as A inner join ISODT010 as B on A.idUsuario = B.idUsuario where B.NombreUsuario = '" + datos.nombreUsuario + "'" +
                "and B.PasswordUsuario = '" + datos.passwordUsuario + "' and idEmpresa =" + datos.idEmpresa;
            Console.WriteLine(con.conexion.CommandText.ToString());
            try
            {
                SqlDataAdapter DA = new SqlDataAdapter();
                DA.SelectCommand = con.conexion;
                DA.Fill(DS);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Dispose();
            }

        }

    }
}
