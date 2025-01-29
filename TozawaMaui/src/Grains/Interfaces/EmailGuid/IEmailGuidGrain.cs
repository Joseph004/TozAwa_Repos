
namespace Grains
{
    public interface IEmailGuidGrain : IGrainWithStringKey
    {
        Task SetAsync(EmailGuidItem item);
        Task ActivateAsync(EmailGuidItem item);
        Task ClearAsync();
        Task<EmailGuidItem> GetAsync();
    }
}