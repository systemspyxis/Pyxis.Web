using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pyxis.Core.Interfaces;


namespace Pyxis.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ValuesController : ControllerBase
    {
        public IDataService DataService;
        public ValuesController(IDataService service)
        {
            DataService = service;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("Query/{filter}")]
        [Produces("application/json")]
        public ActionResult Query(string filter)
        {
            var res = DataService.QueryModel(filter);
            //return Content(JsonConvert.SerializeObject(res));
            return new JsonResult(res);

        }
        [HttpGet("Generate")]
        public ActionResult Generate()
        {
            var res = DataService.GenerateFiles();
            //return Content(JsonConvert.SerializeObject(res));
            return new JsonResult(res);

        }
        

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody]JObject[] value)
        {
            DataService.SaveModel(value.ToList());
            return Content( JsonConvert.SerializeObject(value));
        }

        [HttpPost("Update")]
        public ActionResult Update([FromBody]JObject value)
        {
            var val = JsonConvert.SerializeObject(value);
            DataService.updateModel(value);
            return Content("true");
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
