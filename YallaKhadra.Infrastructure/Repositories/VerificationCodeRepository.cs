using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class VerificationCodeRepository : GenericRepository<VerificationCode>, IVerificationCodeRepository {
        private readonly DbSet<VerificationCode> _verificationCodes;

        public VerificationCodeRepository(AppDbContext context) : base(context) {
            _verificationCodes = context.Set<VerificationCode>();
        }
    }
}
