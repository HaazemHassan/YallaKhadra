using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository {

        private readonly DbSet<RefreshToken> _refreshTokens;


        public RefreshTokenRepository(AppDbContext context) : base(context) {
            _refreshTokens = context.Set<RefreshToken>();
        }

    }
}
