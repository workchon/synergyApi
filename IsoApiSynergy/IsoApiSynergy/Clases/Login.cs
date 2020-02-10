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
            public string dispositivo;
        }


        public bool verificarUsuario(ref DataSet DS, strLogin datos)
        {
            DS = new DataSet();
            con.conexion.CommandText = "select idUsuario, idEmpresa, NombreUsuario, PasswordUsuario, IconoUsuario, Email from [ISO].[dbo].[visEmpresa_Usuario] where NombreUsuario = '" + datos.nombreUsuario + "'" +
                "and PasswordUsuario = '" + datos.passwordUsuario + "' and idEmpresa =" + datos.idEmpresa;
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

        }

        public bool ActualizarAcceso(strLogin datos)
        {
            con.conexion.CommandText = " insert into[ISO].[dbo].[ISOHS001](idUsuario, idEmpresa, Dispositivo, FechaAcceso)"
                + "values('"+datos.idUsuario+"',"+datos.idEmpresa+",'"+datos.dispositivo+ "',GETDATE())";

            try
            {
                con.conexion.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public void Dispose()
        {
            con.Dispose();
        }

    }
}
