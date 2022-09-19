using API.Helper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base
{
    [ServiceFilter(typeof(LogUserActive))]
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {

    }
}