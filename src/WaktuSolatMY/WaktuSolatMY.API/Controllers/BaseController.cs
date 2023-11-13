using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WaktuSolatMY.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}
