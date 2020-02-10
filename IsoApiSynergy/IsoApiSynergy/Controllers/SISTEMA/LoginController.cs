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
                        data = "No sea Encontrado Usuario",
                        Tipo = "200"
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


        


    }
}
