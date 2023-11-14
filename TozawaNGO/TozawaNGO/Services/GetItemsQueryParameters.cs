using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;

namespace TozawaNGO.Services
{
    public class GetItemsQueryParameters
    {
        public string Page { get; set; }
        public string PageSize { get; set; }
        public string IncludeDeleted { get; set; }
        public string SearchString { get; set; }
        public string Email { get; set; }
        public string PageOfEmail { get; set; }

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

        public GetItemsQueryParameters(TableState state, bool includeDeleted, string searchString, string email, string pageOfEmail)
        {
            Page = state.Page.ToString();
            PageSize = state.PageSize.ToString();
            IncludeDeleted = includeDeleted.ToString();
            SearchString = searchString;
            Email = email;
            PageOfEmail = pageOfEmail;
        }
    }
}
