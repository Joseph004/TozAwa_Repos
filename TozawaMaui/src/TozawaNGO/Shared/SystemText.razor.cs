using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShareRazorClassLibrary.Models.Dtos;


namespace TozawaNGO.Shared;

public partial class SystemText : BaseComponent
{
    [Parameter] public Typo SystemTextTypo { get; set; } = Typo.body1;
    [Parameter] public Guid TextId { get; set; }
    [Parameter] public string Style { get; set; } = "font-familly: cursive;";
    [Parameter] public string FallbackText { get; set; }
    [Parameter] public int? Limit { get; set; }
#pragma warning disable CS0108
    [Parameter] public bool IsFirstLoaded { get; set; }
    [Parameter] public bool? ToUpper { get; set; }

    public string NotTranslated = string.Empty;
    public string NotTranslatedTitle = string.Empty;

    private string _translatedText { get; set; }
    private bool _istTranslated { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

    public string Translate()
    {
        var translation = _translationService.Translate(TextId, FallbackText, Limit, ToUpper);
        bool translationChanged = CheckIfIsTranslatedChanged(translation);

        NotTranslated = translation.IsTranslated ? string.Empty : "notTranslated";
        NotTranslatedTitle = translation.IsTranslated ? string.Empty : $"Not translated: {translation.Id}";
        bool newTranslation = CheckIfTranslationChanged(translation);

        if (newTranslation || translationChanged)
        {
            StateHasChanged();
        }

        return translation.Text;
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
}