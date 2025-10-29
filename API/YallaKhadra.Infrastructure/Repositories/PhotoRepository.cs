using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        private readonly DbSet<Photo> _photos;


        public PhotoRepository(AppDbContext context) : base(context)
        {
            _photos = context.Set<Photo>();
        }
    }
}
