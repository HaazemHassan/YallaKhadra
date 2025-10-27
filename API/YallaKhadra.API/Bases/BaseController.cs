﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.API.Bases {
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("defaultLimiter")]
    public class BaseController : ControllerBase {

        protected IMediator mediator;

        public BaseController(IMediator mediator) {
            this.mediator = mediator;

        }

        protected IActionResult NewResult<T>(Response<T> response) {
            switch (response.StatusCode) {
                case HttpStatusCode.OK:
                return new OkObjectResult(response);
                case HttpStatusCode.Created:
                return new CreatedResult(string.Empty, response);
                case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult(response);
                case HttpStatusCode.Forbidden:
                return new ObjectResult(response) { StatusCode = 403 };
                case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(response);
                case HttpStatusCode.NotFound:
                return new NotFoundObjectResult(response);
                case HttpStatusCode.Accepted:
                return new AcceptedResult(string.Empty, response);
                case HttpStatusCode.Conflict:
                return new ConflictObjectResult(response);
                default:
                return new BadRequestObjectResult(response);
            }


        }
        protected bool IsWebClient() {
            return Request.Headers.TryGetValue("X-Client-Type", out var headerValue) && headerValue == "Web";
        }
    }
}
