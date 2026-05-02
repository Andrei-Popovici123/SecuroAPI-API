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
    }
}
