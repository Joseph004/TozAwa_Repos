namespace Tozawa.Bff.Portal.HttpClients
{
    public interface IHttpClientBase
    {
        Task<T> Delete<T>(string uri, CancellationToken cancellationToken);
        Task Delete(string uri, CancellationToken cancellationToken);
        Task<T> Get<T>(string uri);
        Task<T> Get<T>(string uri, CancellationToken cancellationToken);
        Task<T> Patch<T>(string uri, object value, CancellationToken cancellationToken);
        Task<T> Patch<T>(string uri, object value);
        Task<T> Post<T>(string uri, object value, CancellationToken cancellationToken, bool multiContent = false);
        Task<T> Post<T>(string uri, object value, bool multiContent = false);
        Task<T> Put<T>(string uri, object value, CancellationToken cancellationToken);
        Task Put(string uri, object value);
        Task Put(string uri, object value, CancellationToken cancellationToken);
    }
}