using System.Collections.Generic;
using System.Linq;

namespace Tozawa.Language.Svc.Models
{
    public class PagedDto<T> where T : class
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }

    public static class PagedDtoExtensions
    {
        public static PagedDto<T> CreatePagedDto<T>(this IEnumerable<T> entities, int page, int pageSize)
            where T : class
        {
            var result = entities.ToList();

            return new PagedDto<T>
            {
                TotalItems = result.Count,
                Page = page,
                PageSize = pageSize,
                Items = result.Skip((page - 1) * pageSize).Take(pageSize)
            };
        }
    }
}