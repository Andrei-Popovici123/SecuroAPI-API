using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIRegistryController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
