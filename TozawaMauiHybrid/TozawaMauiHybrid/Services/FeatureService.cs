using MudBlazor;
using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services;

public class FeatureService(ITozAwaBffHttpClient client)
{
    private readonly ITozAwaBffHttpClient _client = client;
    private const string _baseUriPath = $"feature";

    public async Task<GetResponse<TableData<FeatureDto>>> GetItems(string page, string pageSize, bool includeDeleted, string searchString, string id)
    {
        var uri = new GetItemsQueryParameters(page, pageSize, includeDeleted, searchString, null, null, id).ToQueryString(_baseUriPath);
        return await _client.SendGet<TableData<FeatureDto>>(uri);
    }
    public async Task<GetResponse<FeatureDto>> GetItem(int id)
    {
        return await _client.SendGet<FeatureDto>($"{_baseUriPath}/{id}");
    }
    public async Task<UpdateResponse> PatchFeature(int id, PatchFeatureRequest request)
    {
        return await _client.SendNoEntityPatch<PatchFeatureRequest>($"{_baseUriPath}/{id}", request.ToPatchDocument());
    }
    public async Task<bool> FeatureExists(int id)
    {
        var response = await GetItems("0", "1000", false, null, id.ToString().Trim());
        return response.Entity?.TotalItems > 0;
    }
    public async Task<AddResponse<FeatureDto>> AddFeature(AddFeatureRequest request)
    {
        return await _client.SendPost<FeatureDto>(_baseUriPath, request);
    }
}
