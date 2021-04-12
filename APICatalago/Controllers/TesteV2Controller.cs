using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalago.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v2/teste")] //indica na URL a versão da API
    [ApiController]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV2Controller - V 2.0 </h2></body></html>", "text/html");
        }


        //[HttpGet, MapToApiVersion("2.0")] // versionamento a nivel de metodo
        //public IActionResult GetVersao2()
        //{
        //    return Content("<html><body><h2>TesteV1Controller - GET V 2.0 </h2></body></html>", "text/html");
        //}
    }
}
