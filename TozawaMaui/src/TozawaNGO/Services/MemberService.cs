using Grains;
using MudBlazor;
using Orleans.Streams;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Models.ResponseRequests;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Services
{
    public class MemberService(ITozAwaBffHttpClient client, IClusterClient clientCluster, ILogger<MemberService> logger)
    {
        private readonly ITozAwaBffHttpClient _client = client;
        private readonly IClusterClient _clientCluster = clientCluster;
        private readonly ILogger<MemberService> _logger = logger;
        private const string _baseUriPath = $"member";

        public async Task<GetResponse<TableData<MemberDto>>> GetItems(string page, string pageSize, bool includeDeleted, string searchString, string pageOfEmail, string email = "")
        {
            var uri = new GetItemsQueryParameters(page, pageSize, includeDeleted, searchString, email, pageOfEmail).ToQueryString(_baseUriPath);
            return await _client.SendGet<TableData<MemberDto>>(uri);
        }
        public async Task<GetResponse<MemberDto>> GetItem(Guid id)
        {
            return await _client.SendGet<MemberDto>($"{_baseUriPath}/{id}");
        }
        public async Task<UpdateResponse> PatchMember(Guid id, PatchMemberRequest request)
        {
            return await _client.SendNoEntityPatch<PatchMemberRequest>($"{_baseUriPath}/{id}", request.ToPatchDocument());
        }

        public async Task<bool> EmailExists(string email)
        {
            email = email.Trim();
            var response = await GetItems("0", "1000", false, null, null, email);
            return response.Entity?.TotalItems > 0;
        }

        public async Task<AddResponse<MemberDto>> AddMember(AddMemberRequest request)
        {
            return await _client.SendPost<MemberDto>(_baseUriPath, request);
        }

        public Task<StreamSubscriptionHandle<MemberNotification>> SubscribeAsync(Guid ownerKey, Func<MemberNotification, Task> action) =>
            _clientCluster.GetStreamProvider("SMS")
                .GetStream<MemberNotification>(ownerKey)
                .SubscribeAsync(new MemberItemObserver(_logger, action));

        private class MemberItemObserver(ILogger<MemberService> logger, Func<MemberNotification, Task> action) : IAsyncObserver<MemberNotification>
        {
            private readonly ILogger<MemberService> _logger = logger;
            private readonly Func<MemberNotification, Task> _action = action;

            public Task OnCompletedAsync() => Task.CompletedTask;

            public Task OnErrorAsync(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Task.CompletedTask;
            }
            public Task OnNextAsync(MemberNotification item, StreamSequenceToken token = null) => _action(item);
        }
    }
}
