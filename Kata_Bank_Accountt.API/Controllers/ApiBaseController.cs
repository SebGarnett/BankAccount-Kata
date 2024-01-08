using Kata_Bank_Account.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kata_Bank_Account.API.Controllers
{
    [ApiController]
    [ApiExceptionFilter]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
