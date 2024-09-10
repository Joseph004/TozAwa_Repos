using Grains.Auth.Models.Authentication;

namespace Grains
{
    public interface ITranslationGrain : IGrainWithGuidKey
    {
        Task SetAsync(TranslationItem item);
        Task ActivateAsync(TranslationItem item);
        Task ClearAsync();
        Task<TranslationItem> GetAsync();
    }
}