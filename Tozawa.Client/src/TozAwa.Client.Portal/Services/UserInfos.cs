using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Tozawa.Client.Portal.AuthenticationServices;
using Tozawa.Client.Portal.Models.Dtos;

namespace Tozawa.Client.Portal.Services
{
public interface IUserInfos
{
    string Sid { get; }
    ActiveCultureDto Culture { get; }
    bool IsAuthenticated { get; }
    BlazorServerAuthData AuthData { get; }
    Task<AuthenticationState> GetAuthenticationStateAsync();
}

public class UserInfos : IUserInfos
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly BlazorServerAuthStateCache _authStateCache;
    private readonly AuthenticationStateProvider _authState;
    private readonly ICookie _cookie;

    public UserInfos(IHttpContextAccessor httpContext, BlazorServerAuthStateCache authStateCache, AuthenticationStateProvider authState, ICookie cookie)
    {
        _httpContext = httpContext;
        _authStateCache = authStateCache;
        _authState = authState;
        _cookie = cookie;
    }

    public BlazorServerAuthData AuthData => _authStateCache.Get(Sid);
    public bool IsAuthenticated => _httpContext.HttpContext.User.Identity.IsAuthenticated;
    public string Sid => Guid.Empty.ToString() /* _httpContext.HttpContext.User.Claims.Where(c => c.Type.Equals("sid")).Select(c => c.Value).FirstOrDefault() */;

    public ActiveCultureDto Culture
    {
        get
        {
            return _authStateCache.GetCulture(Sid) ?? DefaultCulture;
        }
        set
        {
            value ??= DefaultCulture;
            _cookie.SetValue("activeCulture", value.ToCookieString);
            _authStateCache.SetCulture(Sid, value);
        }
    }

    public async Task<AuthenticationState> GetAuthenticationStateAsync() => await _authState.GetAuthenticationStateAsync();
    private static ActiveCultureDto DefaultCulture => new() { Id = Guid.Parse("c7b72512-a6fb-4a9a-b017-01059ccbc717"), LongName = "English", ShortName = "en-US" };
}

}
