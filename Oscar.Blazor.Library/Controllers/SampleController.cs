using Microsoft.AspNetCore.Mvc;

namespace Oscar.Blazor.Library.Controllers;

public class SampleController : Controller
{
    [HttpGet]
    public IActionResult GetData()
    {
        // Your logic to fetch data
        return Ok("Data from controller");
    }

    [HttpPost]
    public IActionResult PostData([FromBody] string model)
    {
        // Your logic to handle posted data
        return Ok("Data posted to controller");
    }
}