
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Tozawa.Client.Portal.Configurations;

namespace TozAwa.Client.Portal.Helpers;

public sealed class CustomMessageHandler : DelegatingHandler
{
    public BrowserRequestCache DefaultBrowserRequestCache { get; set; }
    public BrowserRequestMode DefaultBrowserRequestMode { get; set; }
    public CustomMessageHandler(bool useDependencyInjection = true)
    {
        if (useDependencyInjection)
            return;

        var handler = new HttpClientHandler
        {
            // what ever
        };

        InnerHandler = handler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get the existing options to not override them if set explicitly
        IDictionary<string, object> existingProperties = null;
        if (request.Options.TryGetValue(new HttpRequestOptionsKey<object>("WebAssemblyFetchOptions"), out object fetchOptions))
        {
            existingProperties = (IDictionary<string, object>)fetchOptions;
        }

        if (existingProperties?.ContainsKey("cache") != true)
        {
            request.SetBrowserRequestCache(DefaultBrowserRequestCache);
        }

        if (existingProperties?.ContainsKey("mode") != true)
        {
            request.SetBrowserRequestMode(DefaultBrowserRequestMode);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("CustomMessageHandler catch 401");

            // TODO: some logic (e.g. refreshing token)

            // Or just redirect to login page.
            string loingUrl = "http://some-site/login";
            //_navigationManager.NavigateTo("https://localhost:44331", forceLoad: true);
        }
        return response;
    }
}