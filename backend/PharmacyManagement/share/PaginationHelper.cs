using Microsoft.EntityFrameworkCore;

namespace PharmacyManagement.share
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
    }

    public static class PaginationHelper
    {
        public static int GetSkip(int page, int count)
        {
            return (page - 1) * count;
        }

        public static float GetTotalPage(int numRecords, int count)
        {
            return (float)Math.Ceiling((double)numRecords / count);
        }

        public static async Task<PagedResult<TEntity>> GetPagedAsync<TEntity>(
            Func<int, int, Task<List<TEntity>>> fetchPage,
            Func<Task<int>> countAsync,
            int page, int count)
        {
            int skip = GetSkip(page, count);
            List<TEntity> items = await fetchPage(skip, count);
            int numRecords = await countAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = GetTotalPage(numRecords, count)
            };
        }

        public static async Task<PagedResult<TEntity>> ToPagedResultAsync<TEntity>(
            this IQueryable<TEntity> query, BaseFilterDto filter)
        {
            int numRecords = await query.CountAsync();

            List<TEntity> items = await query
                .Skip(GetSkip(filter.PageNumber, filter.PageSize))
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = GetTotalPage(numRecords, filter.PageSize)
            };
        }
    }
}
