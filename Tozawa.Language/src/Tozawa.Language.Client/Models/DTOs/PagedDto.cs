using System.Collections.Generic;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class PagedDto<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 35;
        public IEnumerable<T> Items { get; set; }
    }
}
