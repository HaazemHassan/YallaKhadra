using Microsoft.AspNetCore.Authorization;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Enums;

public class SameUserOrAdminHandler
    : AuthorizationHandler<SameUserOrAdminRequirement, int> {
    private readonly ICurrentUserService _currentUserService;

    public SameUserOrAdminHandler(ICurrentUserService currentUserService) {
        _currentUserService = currentUserService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameUserOrAdminRequirement requirement,
        int requestedUserId) {
        if (!_currentUserService.IsAuthenticated)
            return Task.CompletedTask;

        if (_currentUserService.IsInRole(UserRole.SuperAdmin) || _currentUserService.IsInRole(UserRole.Admin)) {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (_currentUserService.UserId == requestedUserId) {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}