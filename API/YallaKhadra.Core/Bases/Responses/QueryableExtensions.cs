using Microsoft.EntityFrameworkCore;

namespace YallaKhadra.Core.Bases.Responses {
    public static class QueryableExtensions {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> sourceList, int pageNumber, int pageSize)
            where T : class {
            if (sourceList == null) {
                throw new Exception("Can't paginate empty source");
            }

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            int count = await sourceList.AsNoTracking().CountAsync();
            if (count == 0)
                return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);

            var items = await sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
    }
}
