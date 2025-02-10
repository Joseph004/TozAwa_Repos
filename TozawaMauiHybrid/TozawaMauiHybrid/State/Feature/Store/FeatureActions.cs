using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Services;
using TozawaNGO.Models;

namespace TozawaMauiHybrid.State.Feature.Store;
public record HandleSearchStringFeatureAction(string searchString)
{
    public string SearchString { get; } = searchString;
}
public record HandleIncludeDeletedFeatureAction(string includeDeleted)
{
    public string IncludeDeleted { get; } = includeDeleted;
}
public record FeatureDataFechedAction(FeatureKeyedCollection Features, int totalItems, HubConnection hubConnection);
public record FeatureDataAction(string page, string pageSize, string searchString, bool includeDeleted, double scrollTop, LoadingState loadingState, IJSRuntime jSRuntime, int id);
public record FeaturePatchAction(int id, PatchFeatureRequest request, FeatureDto backItem)
{
    public int Id { get; } = id;
    public PatchFeatureRequest Request { get; } = request;
    public FeatureDto BackItem { get; } = backItem;
}
public record FeatureSelectedAction(FeatureDto selectedItem)
{
    public FeatureDto SelectedItem { get; } = selectedItem;
}
public record FeaturePatchAfterAction(FeatureDto feature);
public record UnSubscribeAction;
public record FeatureAddAction(
string text,
string description,
List<TranslationRequest> textTranslations,
List<TranslationRequest> descriptionTranslations
)
{
    public string Text { get; } = text;
    public string Description { get; } = description;
     public List<TranslationRequest> TextTranslations { get; } = textTranslations;
    public List<TranslationRequest> DescriptionTranslations { get; } = descriptionTranslations;
}
public record ScrollTopAction(double scrollTop)
{
    public double ScrollTop { get; } = scrollTop;
}
public record FeatureAddAfterAction(FeatureDto feature);
public class LoadItemAction(int id, bool isUpdated)
{
    public int Id { get; } = id;
    public bool IsUpdated { get; } = isUpdated;
}
public class RemoveItemAction(int id)
{
    public int Id { get; set; } = id;
}
public class LoadDataAction { }
