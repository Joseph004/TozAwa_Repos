using System;
using System.Collections.Concurrent;
using Tozawa.Client.Portal.Models.Dtos;

namespace Tozawa.Client.Portal.AuthenticationServices
{
public class BlazorServerAuthStateCache
{
    private readonly ConcurrentDictionary<string, BlazorServerAuthData> Cache = new();
    private readonly ConcurrentDictionary<string, ActiveCultureDto> UserLanguages = new();
    public bool HasSubjectId(string subjectId) => Cache.ContainsKey(subjectId);

    public void Add(string subjectId, DateTimeOffset expiration, string idToken, string accessToken, string refreshToken, DateTimeOffset refreshAt)
    {
        System.Diagnostics.Debug.WriteLine($"Caching sid: {subjectId}");

        var data = new BlazorServerAuthData
        {
            SubjectId = subjectId,
            Expiration = expiration,
            IdToken = idToken,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshAt = refreshAt
        };
        Cache.AddOrUpdate(subjectId, data, (k, v) => data);
    }

    public BlazorServerAuthData Get(string subjectId)
    {
        Cache.TryGetValue(subjectId, out var data);
        return data;
    }

    public ActiveCultureDto GetCulture(string subjectId)
    {
        UserLanguages.TryGetValue(subjectId, out var data);
        return data;
    }

    public void SetCulture(string subjectId, ActiveCultureDto culture)
        => UserLanguages.AddOrUpdate(subjectId, culture, (k, v) => culture);

    public void Remove(string subjectId)
     => Cache.TryRemove(subjectId, out _);
}
}

