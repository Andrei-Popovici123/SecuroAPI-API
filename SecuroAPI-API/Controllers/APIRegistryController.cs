using Microsoft.AspNetCore.Mvc;


namespace SecuroAPI_API.Controllers;

public class APIRegistryController : Controller
{
    // GET
    public IActionResult Index()
    { 
        return View();
    }
}