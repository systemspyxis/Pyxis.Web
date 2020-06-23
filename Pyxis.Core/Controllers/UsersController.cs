using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Pyxis.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pyxis.Core.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public IAuthenticationService AuthService;
        public UsersController(IAuthenticationService service)
        {
            AuthService = service;
        }
        // GET: api/<controller>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult Get()
        {
            var res = AuthService.GetUsers();
            return new JsonResult(res);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPost("Create")]
        public void Create([FromBody]JObject value)
        {
            AuthService.CreateUser(value);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
