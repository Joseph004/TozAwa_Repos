
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using TozawaNGO.Models;

namespace TozawaNGO.Services;
public class BlazorServerAuthStateCache(ISessionStorageService sessionStorageService)
{
    private ISessionStorageService _sessionStorageService { get; set; } = sessionStorageService;

    public async Task<bool> HasObjectId(string oid)
    {
        var key = $"auth-{oid}";
        return await _sessionStorageService.GetItemAsync<BlazorServerAuthData>(key) != null;
    }

    public async Task Add(string oid, string accessToken, string refreshToken, DateTimeOffset refreshAt)
    {
        if (await HasObjectId(oid))
        {
            await AddAccessToken(oid, accessToken);
            return;
        }

        var data = new BlazorServerAuthData
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshAt = refreshAt
        };

        await _sessionStorageService.SetItemAsync($"auth-{oid}", data);
    }

    public async Task AddAccessToken(string oid, string accessToken)
    {
        var cache = await Get(oid);
        cache.AccessToken = accessToken;
        await _sessionStorageService.RemoveItemAsync($"auth-{oid}");
        await _sessionStorageService.SetItemAsync($"auth-{oid}", cache);
    }

    public async Task Update(string oid, BlazorServerAuthData data)
    {
        if (await HasObjectId(oid))
        {
            await _sessionStorageService.RemoveItemAsync($"auth-{oid}");
            await _sessionStorageService.SetItemAsync($"auth-{oid}", data);
        }
    }

    public async Task<BlazorServerAuthData> Get(string oid)
    {
        if (!await HasObjectId(oid))
        {
            return null;
        }

        var data = await _sessionStorageService.GetItemAsync<BlazorServerAuthData>($"auth-{oid}");
        return data;
    }

    public async Task Remove(string oid)
    {
        if (await HasObjectId(oid))
        {
            await _sessionStorageService.RemoveItemAsync($"auth-{oid}");
        }
    }
}
