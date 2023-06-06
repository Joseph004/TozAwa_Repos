
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Tozawa.Client.Portal.HttpClients.Helpers;

public sealed class DefaultBrowserOptionsMessageHandler : DelegatingHandler
{
    public DefaultBrowserOptionsMessageHandler()
    {
    }

    public DefaultBrowserOptionsMessageHandler(HttpMessageHandler innerHandler)
    {
        InnerHandler = innerHandler;
    }

    public BrowserRequestCache DefaultBrowserRequestCache { get; set; }
    public BrowserRequestCredentials DefaultBrowserRequestCredentials { get; set; }
    public BrowserRequestMode DefaultBrowserRequestMode { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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

        if (existingProperties?.ContainsKey("credentials") != true)
        {
            request.SetBrowserRequestCredentials(DefaultBrowserRequestCredentials);
        }

        if (existingProperties?.ContainsKey("mode") != true)
        {
            request.SetBrowserRequestMode(DefaultBrowserRequestMode);
        }
        request.Headers.Add("Accept", "application/x-www-form-urlencoded");
        request.Headers.Add("Access-Control-Allow-Origin", "https://localhost:44331");
        return base.SendAsync(request, cancellationToken);
    }
}