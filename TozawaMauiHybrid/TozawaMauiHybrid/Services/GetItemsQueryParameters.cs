using Microsoft.AspNetCore.WebUtilities;

namespace TozawaMauiHybrid.Services
{
    public class GetItemsQueryParameters(string page, string pageSize, bool includeDeleted, string searchString, string email, string pageOfEmail, string id)
    {
        public string Page { get; set; } = page;
        public string PageSize { get; set; } = pageSize;
        public string IncludeDeleted { get; set; } = includeDeleted.ToString();
        public string SearchString { get; set; } = searchString;
        public string Email { get; set; } = email;
        public string Id { get; set; } = id;
        public string PageOfEmail { get; set; } = pageOfEmail;

        public string ToQueryString(string basePath)
        {
            var uri = basePath;

            foreach (var propertyInfo in GetType().GetProperties())
            {
                var propertyName = propertyInfo.Name;
                var propertyValue = propertyInfo.GetValue(this);
                if (propertyValue == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(propertyValue.ToString()))
                {
                    uri = QueryHelpers.AddQueryString(uri, propertyName, propertyValue.ToString());
                }
            }

            return uri;
        }
    }
}
