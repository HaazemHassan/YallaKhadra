using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace YallaKhadra.API.Filters {


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AnonymousOnlyAttribute : Attribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationFilterContext context) {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true) {
                context.Result = new ObjectResult(new {
                    succeeded = false,
                    message = "This endpoint is only accessible to anonymous users",
                    statusCode = StatusCodes.Status403Forbidden
                }) {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
