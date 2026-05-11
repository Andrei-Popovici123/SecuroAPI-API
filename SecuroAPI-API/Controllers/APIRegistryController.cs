using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIRegistryController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<string> list = ["value1","value2"];
             return Ok(list);
        }
        
        [HttpGet("{id}")]
        public string GetByID(int id)
        {
            return "value";
        }
        [HttpPost]
        public void Post([FromBody]string value){}

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {}
        
        [HttpDelete("{id}")]
        public void Delete(int id){}
    }
}
