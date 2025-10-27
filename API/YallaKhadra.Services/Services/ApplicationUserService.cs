using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services {
    public class ApplicationUserService : IApplicationUserService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IEmailService _emailsService;
        private readonly IUrlHelper _urlHelper;

        public ApplicationUserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor /*IUrlHelperFactory urlHelperFactory*/ ) {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            //_urlHelper = urlHelper;
        }

        public async Task<ServiceOperationResult<ApplicationUser?>> AddUser(ApplicationUser user, string password) {
            if (user is null || password is null)
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.InvalidParameters, "User or password is invalid");
            
            if (await IsUserExist(x => x.Email == user.Email))
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.AlreadyExists, "Email already exists");

            if (await IsUserExist(x => x.UserName == user.UserName))
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.AlreadyExists, "This username is used");

            if (await IsUserExist(x => x.PhoneNumber == user.PhoneNumber))
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.AlreadyExists, "This phone number is used");

            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.Failed, "Failed to create user");

            var addToRoleresult = await _userManager.AddToRoleAsync(user, ApplicationUserRole.User.ToString());
            if (!addToRoleresult.Succeeded)
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.Failed, "Failed to assign user role");

            //var succedded = await SendConfirmationEmailAsync(user);
            //if (!succedded)
            //    return ServiceOperationResult.Failed;
            
            return ServiceOperationResult<ApplicationUser?>.Success(user);
        }


        public async Task<bool> IsUserExist(Expression<Func<ApplicationUser, bool>> predicate) {
            var user = await _userManager.Users.FirstOrDefaultAsync(predicate);
            return user is null ? false : true;
        }



        //public async Task<bool> SendConfirmationEmailAsync(ApplicationUser user) {
        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var resquestAccessor = _httpContextAccessor.HttpContext.Request;
        //    var confrimEmailActionContext = new UrlActionContext {
        //        Action = "ConfirmEmail",
        //        Controller = "Authentication",
        //        Values = new { UserId = user.Id, Code = code }
        //    };
        //    var returnUrl = resquestAccessor.Scheme + "://" + resquestAccessor.Host + _urlHelper.Action(confrimEmailActionContext);
        //    var message = $"To Confirm Email Click Link: {returnUrl}";
        //    var sendResult = await _emailsService.SendEmail(user.Email, message, "Confirm email");
        //    return sendResult;
        //}

        //public async Task<ServiceOperationResult<string?>> ConfirmEmailAsync(int userId, string code) {
        //    if (code is null)
        //        return ServiceOperationResult<string>.Failure(ServiceOperationStatus.InvalidParameters, "Confirmation code is required");

        //    var user = await _userManager.FindByIdAsync(userId.ToString());
        //    if (user is null)
        //        return ServiceOperationResult<string>.Failure(ServiceOperationStatus.NotFound, "User not found");

        //    if (user.EmailConfirmed)
        //        return ServiceOperationResult<string>.Failure(ServiceOperationStatus.Failed, "Email already confirmed");

        //    var confirmEmail = await _userManager.ConfirmEmailAsync(user, code);
        //    return confirmEmail.Succeeded
        //        ? ServiceOperationResult<string>.Success("Email confirmed successfully")
        //        : ServiceOperationResult<string>.Failure(ServiceOperationStatus.Failed, "Failed to confirm email");

        //}

        //public async Task<ServiceOperationResult<string?>> ResetPasswordAsync(ApplicationUser? user, string newPassword) {
        //    if (user is null || newPassword is null)
        //        return ServiceOperationResult<string>.Failure(ServiceOperationStatus.InvalidParameters, "User or password is invalid");

        //    await using var trans = await _unitOfWork.BeginTransactionAsync();
        //    try {
        //        await _userManager.RemovePasswordAsync(user);
        //        var result = await _userManager.AddPasswordAsync(user, newPassword);
        //        if (!result.Succeeded) {
        //            await _unitOfWork.RollbackAsync();
        //            return ServiceOperationResult<string>.Failure(ServiceOperationStatus.Failed, "Failed to reset password");
        //        }

        //        await _unitOfWork.CommitAsync();
        //        return ServiceOperationResult<string>.Success("Password reset successfully");
        //    }
        //    catch (Exception ex) {
        //        await _unitOfWork.RollbackAsync();
        //        return ServiceOperationResult<string>.Failure(ServiceOperationStatus.Failed, $"An error occurred: {ex.Message}");

        //    }

        //}

        public async Task<string?> GetFullName(int userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return null;

            return user.FirstName + " " + user.LastName;
        }

        public async Task<ApplicationUser?> GetUser(Expression<Func<ApplicationUser, bool>> predicate) {
            return await _userManager.Users.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<ApplicationUser>> SearchUsers(Expression<Func<ApplicationUser, bool>> predicate) {
            return await _userManager.Users.Where(predicate).ToListAsync();
        }

        public async Task<List<TResult>> GetUsersListAsync<TResult>(Expression<Func<ApplicationUser, bool>>? filter = null, IConfigurationProvider? config = null) {
            var query = _userManager.Users.AsQueryable();
            if (filter != null) {
                query = query.Where(filter);
            }
            if (config != null) {
                return await query.ProjectTo<TResult>(config).ToListAsync();
            }
            return (await query.ToListAsync()).Cast<TResult>().ToList();
        }


    }
}
