using Grains;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using Orleans.Streams;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Services;

namespace TozawaNGO.State.Member.Store;
public record HandleSearchStringMemberAction(string searchString)
{
    public string SearchString { get; } = searchString;
}
public record HandleIncludeDeletedMemberAction(string includeDeleted)
{
    public string IncludeDeleted { get; } = includeDeleted;
}
public record MemberDataFechedAction(MemberKeyedCollection members, StreamSubscriptionHandle<MemberNotification> subscription, List<MemberNotification> notifications, int totalItems, HubConnection hubConnection);
public record MemberDataAction(string page, string pageSize, string searchString, bool includeDeleted, string pageOfEmail, string email, double scrollTop, LoadingState loadingState, IJSRuntime jSRuntime);
public record MemberPatchAction(Guid id, PatchMemberRequest request, MemberDto backItem)
{
    public Guid Id { get; } = id;
    public PatchMemberRequest Request { get; } = request;
    public MemberDto BackItem { get; } = backItem;
}
public record MemberSelectedAction(MemberDto selectedItem)
{
    public MemberDto SelectedItem { get; } = selectedItem;
}
public record MemberPatchAfterAction(MemberDto member);
public record UnSubscribeAction;
public record MemberAddAction(
string email,
string firsName,
string lastName,
string description,
List<TranslationRequest> descriptionTranslations
)
{
    public string Email { get; } = email;
    public string FirstName { get; } = firsName;
    public string LastName { get; } = lastName;
    public string Description { get; } = description;
    public List<TranslationRequest> DescriptionTranslations { get; } = descriptionTranslations;
}
public record ScrollTopAction(double scrollTop)
{
    public double ScrollTop { get; } = scrollTop;
}
public record MemberAddAfterAction(MemberDto member);
public class LoadItemAction(Guid id, bool isUpdated)
{
    public Guid Id { get; } = id;
    public bool IsUpdated { get; } = isUpdated;
}
public class RemoveItemAction(Guid id)
{
    public Guid Id { get; set; } = id;
}
public class LoadDataAction { }
