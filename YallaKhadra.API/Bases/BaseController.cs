using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.API.Bases {
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("defaultLimiter")]
    public class BaseController : ControllerBase {

        private IMediator _mediatorInstance;
        protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>()!;


        #region Actions
        protected ObjectResult NewResult<T>(Response<T> response) {
            switch (response.StatusCode) {
                case HttpStatusCode.OK:
                return new OkObjectResult(response);
                case HttpStatusCode.Created:
                return new CreatedResult(string.Empty, response);
                case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult(response);
                case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(response);
                case HttpStatusCode.NotFound:
                return new NotFoundObjectResult(response);
                case HttpStatusCode.Accepted:
                return new AcceptedResult(string.Empty, response);
                case HttpStatusCode.UnprocessableEntity:
                return new UnprocessableEntityObjectResult(response);
                default:
                return new BadRequestObjectResult(response);
            }
        }



        protected ObjectResult NewResult(Response response) {
            switch (response.StatusCode) {
                case HttpStatusCode.OK:
                return new OkObjectResult(response);
                case HttpStatusCode.Created:
                return new CreatedResult(string.Empty, response);
                case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult(response);
                case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(response);
                case HttpStatusCode.NotFound:
                return new NotFoundObjectResult(response);
                case HttpStatusCode.Accepted:
                return new AcceptedResult(string.Empty, response);
                case HttpStatusCode.UnprocessableEntity:
                return new UnprocessableEntityObjectResult(response);
                default:
                return new BadRequestObjectResult(response);
            }
        }
        #endregion


    }

}

