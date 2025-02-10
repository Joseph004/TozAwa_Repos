using Microsoft.Extensions.Primitives;

namespace Grains.Auth.Controllers
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
        public bool ByPassOrganization { get; } = false;

        public BaseQuery(Dictionary<string, StringValues> queryParameters = null)
        {
            if (queryParameters == null) return;
            if (queryParameters.TryGetValue(nameof(Page), out StringValues pageValue) && int.TryParse(pageValue, out var page))
            {
                Page = page;
            }

            if (queryParameters.TryGetValue(nameof(PageSize), out StringValues valueSize) && int.TryParse(valueSize, out var pageSize))
            {
                PageSize = pageSize;
            }
            if (queryParameters.TryGetValue(nameof(SearchString), out StringValues searchString) && !string.IsNullOrEmpty(searchString))
            {
                SearchString = searchString;
            }
            if (queryParameters.TryGetValue(nameof(PageOfCode), out StringValues pageOfCode) && !string.IsNullOrEmpty(pageOfCode))
            {
                PageOfCode = pageOfCode;
            }
            if (queryParameters.TryGetValue(nameof(Code), out StringValues code) && !string.IsNullOrEmpty(code))
            {
                Code = code;
            }
            if (queryParameters.TryGetValue(nameof(PageOfEmail), out StringValues pageOfEmail) && !string.IsNullOrEmpty(pageOfEmail))
            {
                PageOfEmail = pageOfEmail;
            }
            if (queryParameters.TryGetValue(nameof(Email), out StringValues email) && !string.IsNullOrEmpty(email))
            {
                Email = email;
            }
            if (queryParameters.TryGetValue(nameof(IncludeDeleted), out StringValues valueIncludeDeleted) && bool.TryParse(valueIncludeDeleted, out var includeDeleted))
            {
                IncludeDeleted = includeDeleted;
            }
            if (queryParameters.TryGetValue(nameof(ByPassOrganization), out StringValues valueByPass) && bool.TryParse(valueByPass, out var byPassOrganization))
            {
                ByPassOrganization = byPassOrganization;
            }
        }
    }
}