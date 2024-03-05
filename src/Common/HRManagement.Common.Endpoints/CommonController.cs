using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Common.Endpoints;

[Route(BaseApiPath)]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class CommonController : ControllerBase
{
    protected const string BaseApiPath = "api/v1";
    private IMediator _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

}