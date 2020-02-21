using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json;
using IsoApiSynergy.Clases;
using Newtonsoft.Json.Linq;

namespace IsoApiSynergy.Controllers.SISTEMA
{
    [Route("SISTEMA/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("Acceso")]
        public ActionResult Acceso([FromBody] Login.strLogin datos)
        {
            Login Login = new Login();
            DataSet DS = new DataSet();
            if (Login.verificarUsuario(ref DS, datos))
            {
                bool acceso = false;
                if(JsonConvert.SerializeObject(DS).Length >= 14)
                {
                    var data = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(DS));
                    var IdUsuario  = data.Descendants()
                                    .OfType<JProperty>()
                                    .FirstOrDefault(x => x.Name == "idUsuario")
                                    ?.Value;
                    datos.idUsuario = int.Parse(IdUsuario.ToString());
                    acceso = Login.ActualizarAcceso(datos);
                    Login.Dispose();
                }
                if (acceso)
                {
                    return Ok
                (
                    new
                    {
                        data = JsonConvert.SerializeObject(DS),
                        Tipo = "200"
                    }
                );
                }
                return Ok
                (
                    new
                    {
                        data = "No se ha encontrado el Usuario",
                        Tipo = "202"
                    }
                );
            }
            else
            {
                return Ok
                (
                    new
                    {
                        data = "Ha ocurrido un error",
                        Tipo = "500"
                    }
                );
            }

        }


        [HttpGet]
        [Route("Recuperar")]
        public ActionResult EnviarCorreo([FromBody] Login.strLogin datos)
        {

            clsMail mail = new clsMail();
            var lista = new List<string>()
            {
               datos.Email
            };
            if (mail.SMTPMail_recuperacion( "Requperacion de contraseña", " Recuperacion de contraseña", lista, "pruebachon@outlook.com", "Jesus Chon" ))
            {

                return Ok
                    (
                        new
                        {
                            data = "Correo enviado Exitosamente",
                            Tipo = "200"
                        }
                    );
            }else
            {

                return Ok
                    (
                        new
                        {
                            data = "Error al enviar correo ",
                            Tipo = "202"
                        }
                    );
            }

        }

        


    }
}
