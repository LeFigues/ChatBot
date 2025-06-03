using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ufl_erp_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
