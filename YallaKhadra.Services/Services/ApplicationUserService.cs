using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services {
    public class ApplicationUserService : IApplicationUserService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientContextService _clientContextService;


        public ApplicationUserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IClientContextService clientContextService) {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _clientContextService = clientContextService;
        }

        public async Task<ServiceOperationResult<ApplicationUser?>> AddUser(ApplicationUser user, string password) {
            user.UserName = user.Email;

            if (await IsUserExist(x => x.Email == user.Email))
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.AlreadyExists, "Email already exists.");


            if (await IsUserExist(x => x.PhoneNumber == user.PhoneNumber))
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.AlreadyExists, "This phone number is used.");

            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return ServiceOperationResult<ApplicationUser?>.Failure(ServiceOperationStatus.Failed, "Failed to create user. Please try again later.");

            //var succedded = await SendConfirmationEmailAsync(user);
            //if (!succedded)
            //    return ServiceOperationResult.Failed;

            return ServiceOperationResult<ApplicationUser?>.Success(user);
        }


        public async Task<bool> IsUserExist(Expression<Func<ApplicationUser, bool>> predicate) {
            var user = await _userManager.Users.FirstOrDefaultAsync(predicate);
            return user is null ? false : true;
        }



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
