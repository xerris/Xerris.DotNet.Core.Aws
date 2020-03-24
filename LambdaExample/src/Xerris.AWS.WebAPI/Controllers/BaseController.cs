using Microsoft.AspNetCore.Mvc;
using Xerris.AWS.Services;

namespace Xerris.AWS.WebAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly IApplicationConfig config;

        protected BaseController(IApplicationConfig config)
        {
            this.config = config;
        }

    }
}