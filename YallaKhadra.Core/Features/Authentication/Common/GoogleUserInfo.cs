namespace YallaKhadra.Core.Features.Authentication.Common {
    public record GoogleUserInfo(
     string GoogleId,
     string Email,
     string FirstName,
     string LastName,
     bool IsEmailVerified
 );
}
