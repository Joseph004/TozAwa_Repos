using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;


namespace TozawaMauiHybrid.Component;

public partial class SystemText : BaseComponent<SystemText>
{
    [Parameter] public Typo SystemTextTypo { get; set; } = Typo.body1;
    [Parameter] public Guid TextId { get; set; }
    [Parameter] public string Style { get; set; } = "font-familly: cursive;";
    [Parameter] public string FallbackText { get; set; }
    [Parameter] public int? Limit { get; set; }
    [Parameter] public bool? ToUpper { get; set; }

    [Inject] FirstloadState FirstloadState { get; set; }

    public string NotTranslated = string.Empty;
    public string NotTranslatedTitle = string.Empty;

    private string _translatedText { get; set; }
    private bool _istTranslated { get; set; }
    private bool _firstLoaded = false;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    private void FirsLoadChanged()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _firstLoaded = true;
            await InvokeAsync(() =>
         {
             StateHasChanged();
         });
            FirstloadState.SetFirsLoad(true);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
    public string Translate()
    {
        var response = "Not translated";
        if (_translationService.TranslationLoaded())
        {
            var translation = _translationService.Translate(TextId, FallbackText, Limit, ToUpper);
            bool translationChanged = CheckIfIsTranslatedChanged(translation);

            NotTranslated = translation.IsTranslated ? string.Empty : "notTranslated";
            NotTranslatedTitle = translation.IsTranslated ? string.Empty : $"Not translated: {translation.Id}";
            bool newTranslation = CheckIfTranslationChanged(translation);

            if (newTranslation || translationChanged)
            {
                InvokeAsync(() =>
        {
            StateHasChanged();
        });
            }
            response = translation.Text;
        }
        return response;
    }

    private bool CheckIfTranslationChanged(TranslationDto translation)
    {
        var newTranslation = translation.Text != _translatedText;
        _translatedText = translation.Text;
        return newTranslation;
    }

    private bool CheckIfIsTranslatedChanged(TranslationDto translation)
    {
        var translationChanged = translation.IsTranslated != _istTranslated;
        _istTranslated = translation.IsTranslated;
        return translationChanged;
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}