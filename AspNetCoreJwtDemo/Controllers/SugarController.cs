using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreJwtDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SugarController : ControllerBase
    {
        [HttpGet,Route("ctl")]
        
        public IActionResult Chotolate()
        {
            return Ok("Chotolate");
        }

        [HttpGet, Route("fudge")]
        [Authorize]
        public IActionResult Fudge()
        {
            return Ok("Fudge");
        }

        [HttpGet, Route("crystalSugar")]
        public IActionResult crystalSugar()
        {
            return Ok("crystal sugar");
        }
    }
}