using Microsoft.EntityFrameworkCore;

namespace YallaKhadra.Core.Bases.Responses {
    public static class QueryableExtensions {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> sourceList, int? pageNumber = 1, int? pageSize = 10)
            where T : class {
            if (sourceList == null) {
                throw new Exception("Can't paginate empty source");
            }


            pageNumber = pageNumber.GetValueOrDefault(1);
            pageSize = pageSize.GetValueOrDefault(10);

            pageNumber = Math.Max(1, pageNumber.Value);
            pageSize = Math.Max(1, pageSize.Value);

            int count = await sourceList.AsNoTracking().CountAsync();
            if (count == 0)
                return PaginatedResult<T>.Success(new List<T>(), count, pageNumber.Value, pageSize.Value);

            var items = await sourceList.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber.Value, pageSize.Value);
        }
    }
}
