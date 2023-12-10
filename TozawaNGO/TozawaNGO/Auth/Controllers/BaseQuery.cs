using Microsoft.Extensions.Primitives;

namespace TozawaNGO.Auth.Controllers
{
    public class BaseQuery
    {
        public int Page { get; } = 1;
        public int PageSize { get; } = 20;
        public string SearchString { get; } = null;
        public string PageOfCode { get; } = null;
        public string Code { get; } = null;
        public string PageOfEmail { get; } = null;
        public string Email { get; } = null;
        public bool IncludeDeleted { get; }

        public BaseQuery(Dictionary<string, StringValues> queryParameters = null)
        {
            if (queryParameters == null) return;
            if (queryParameters.ContainsKey(nameof(Page)) && int.TryParse(queryParameters[nameof(Page)], out var page))
            {
                Page = page;
            }

            if (queryParameters.ContainsKey(nameof(PageSize)) && int.TryParse(queryParameters[nameof(PageSize)], out var pageSize))
            {
                PageSize = pageSize;
            }
            if (queryParameters.ContainsKey(nameof(SearchString)) && !string.IsNullOrEmpty(queryParameters[nameof(SearchString)]))
            {
                SearchString = queryParameters[nameof(SearchString)];
            }
            if (queryParameters.ContainsKey(nameof(PageOfCode)) && !string.IsNullOrEmpty(queryParameters[nameof(PageOfCode)]))
            {
                PageOfCode = queryParameters[nameof(PageOfCode)];
            }
            if (queryParameters.ContainsKey(nameof(Code)) && !string.IsNullOrEmpty(queryParameters[nameof(Code)]))
            {
                Code = queryParameters[nameof(Code)];
            }
            if (queryParameters.ContainsKey(nameof(PageOfEmail)) && !string.IsNullOrEmpty(queryParameters[nameof(PageOfEmail)]))
            {
                PageOfEmail = queryParameters[nameof(PageOfEmail)];
            }
            if (queryParameters.ContainsKey(nameof(Email)) && !string.IsNullOrEmpty(queryParameters[nameof(Email)]))
            {
                Email = queryParameters[nameof(Email)];
            }
            if (queryParameters.ContainsKey(nameof(IncludeDeleted)) && bool.TryParse(queryParameters[nameof(IncludeDeleted)], out var includeDeleted))
            {
                IncludeDeleted = includeDeleted;
            }
        }
    }
}