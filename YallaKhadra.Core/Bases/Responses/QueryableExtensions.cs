using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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

            if (sourceList.Provider is IAsyncQueryProvider) {
                int count = await sourceList.AsNoTracking().CountAsync();
                if (count == 0)
                    return PaginatedResult<T>.Success(new List<T>(), count, pageNumber.Value, pageSize.Value);

                var items = await sourceList.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
                return PaginatedResult<T>.Success(items, count, pageNumber.Value, pageSize.Value);
            }

            var inMemoryList = sourceList.ToList();
            int inMemoryCount = inMemoryList.Count;
            if (inMemoryCount == 0)
                return PaginatedResult<T>.Success(new List<T>(), inMemoryCount, pageNumber.Value, pageSize.Value);

            var inMemoryItems = inMemoryList.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            return PaginatedResult<T>.Success(inMemoryItems, inMemoryCount, pageNumber.Value, pageSize.Value);
        }
    }
}
