
using Microsoft.AspNetCore.Components;

namespace TozawaNGO.Shared
{
    public partial class RightSideMenu : BaseComponent<RightSideMenu>
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        private (bool isOpen, int leftSize, int rightSize) _value;
        [Parameter]
        public (bool isOpen, int leftSize, int rightSize) Settings
        {
            get => _value;
            set
            {
                if (_value.leftSize == value.leftSize && _value.isOpen == value.isOpen) return;
                _value = value;
                _leftSize = _value.leftSize;
                _isMenuOpen = _value.isOpen;
                StateHasChanged();
            }
        }
        private int _leftSize = 97;
        private bool _isMenuOpen = true;
        protected override async Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            await base.OnInitializedAsync();
        }
        private async void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
           {
               StateHasChanged();
           });
        }

        protected override void Dispose(bool disposed)
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            base.Dispose(disposed);
        }

    }
}

