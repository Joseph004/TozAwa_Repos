
using Microsoft.AspNetCore.Components;

namespace TozawaNGO.Shared
{
    public partial class LeftSideMenu : BaseComponent<RightSideMenu>
    {
        private (bool isOpen, int leftSize, int rightSize) _value;
        private int _padding = 60;
        private int _top = 0;
        private string _textAllign = "center";
        private string _position = "";
        private string _leftSideMenuOutside = "";
        [Parameter]
        public (bool isOpen, int leftSize, int rightSize) Settings
        {
            get => _value;
            set
            {
                if (_value.rightSize == value.rightSize && _value.leftSize == value.leftSize && _value.isOpen == value.isOpen) return;
                _value = value;
                _rightSize = _value.rightSize;
                _isMenuOpen = _value.isOpen;
                if (!_isMenuOpen)
                {
                    _top = 75;
                    _padding = 0;
                    _position = "fixed";
                    _leftSideMenuOutside = "leftSideMenuOutside";
                }
                StateHasChanged();
            }
        }
        private int _rightSize = 97;
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

