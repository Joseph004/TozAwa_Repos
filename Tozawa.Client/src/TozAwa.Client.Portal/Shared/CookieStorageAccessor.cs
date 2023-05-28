using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Tozawa.Client.Portal.Shared
{
    public class CookieStorageAccessor
    {
        private Lazy<IJSObjectReference> _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public CookieStorageAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task WaitForReference()
        {
            if (_accessorJsRef.IsValueCreated is false)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/CookieStorageAccessor.js"));
            }
        }

        public async Task<T> GetValueAsync<T>(string key, string encryptionKey = "")
        {
            await WaitForReference();
            var result = await _accessorJsRef.Value.InvokeAsync<T>("get", key);

            if (!string.IsNullOrEmpty(encryptionKey))
            {
                result = await getDecryptedValueAsync(result, encryptionKey);
            }

            return result;
        }

        public async Task<T> getDecryptedValueAsync<T>(T value, string encryptionKey = "none")
        {
            await WaitForReference();
            var result = await _accessorJsRef.Value.InvokeAsync<T>("getDecrypted", value, encryptionKey);

            return result;
        }

        public async Task SetValueAsync<T>(string key, T value, bool useEncryption = false, string encryptionKey = "none")
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("set", key, value, useEncryption, encryptionKey);
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }
    }
}