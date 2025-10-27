using Microsoft.AspNetCore.Http;

namespace YallaKhadra.Core.Abstracts.ServicesContracts;

public interface ITokenService {
    public CookieOptions GetRefreshTokenCookieOptions();
}
