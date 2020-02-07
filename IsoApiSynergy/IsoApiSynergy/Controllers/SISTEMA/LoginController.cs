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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
                return Ok
                (
                    new
                    {
                        data = JsonConvert.SerializeObject(DS),
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
