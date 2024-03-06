using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;

namespace Grains.Services
{
    public class GetItemsQueryParameters(TableState state, bool includeDeleted, string searchString, string email, string pageOfEmail)
    {
        public string Page { get; set; } = state.Page.ToString();
        public string PageSize { get; set; } = state.PageSize.ToString();
        public string IncludeDeleted { get; set; } = includeDeleted.ToString();
        public string SearchString { get; set; } = searchString;
        public string Email { get; set; } = email;
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
