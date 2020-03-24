using System.Reflection.Emit;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Xerris.AWS.Services;
using Xerris.AWS.Services.Services;

namespace Xerris.AWS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : BaseController
    {
        private readonly IHelloService service;

        public HelloController(IHelloService service, IApplicationConfig config) : base(config)
        {
            this.service = service;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetHelloList([FromQuery]string salesManagerId, [FromQuery] string searchString,
            [FromQuery] string sourceContext)
        {
            Log.Debug("/api/hello called");
            return Ok("hello");
        }
    }
}