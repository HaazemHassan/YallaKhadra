namespace YallaKhadra.API.Middlewares {
    public class GuestSessionMiddleware {
        private readonly RequestDelegate _next;
        private const string GuestCookieName = "guest_session_id";

        public GuestSessionMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            if (context.Request.Cookies.TryGetValue(GuestCookieName, out var guestId) && !string.IsNullOrEmpty(guestId)) {
                context.Items["GuestId"] = guestId;
            }
            else {
                var newGuestId = Guid.NewGuid().ToString();

                var cookieOptions = new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(90),
                    SameSite = SameSiteMode.Lax
                };

                context.Response.Cookies.Append(GuestCookieName, newGuestId, cookieOptions);
                context.Items["GuestId"] = newGuestId;
            }

            await _next(context);
        }
    }
}
