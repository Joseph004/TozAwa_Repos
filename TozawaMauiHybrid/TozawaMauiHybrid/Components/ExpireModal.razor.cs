using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TozawaMauiHybrid.Component
{
    public partial class ExpireModal : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Title { get; set; }

        private static System.Timers.Timer aTimer;
        private int Counter = 60;

        protected async override Task OnInitializedAsync()
        {
            aTimer = new System.Timers.Timer(2000);
            aTimer.Elapsed += CountDownTimer;
            aTimer.Enabled = true;

            await base.OnInitializedAsync();
        }

        public void CountDownTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (Counter > 0)
            {
                Counter -= 1;
            }
            else
            {
                aTimer.Enabled = false;
                Confirm();
            }
            InvokeAsync(StateHasChanged);
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Confirm()
        {
            InvokeAsync(async () =>
          {
              MudDialog.Close(DialogResult.Ok(true));
              await Task.CompletedTask;
          });
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            if (aTimer != null)
            {
                aTimer.Elapsed -= CountDownTimer;
            }
        }
    }
}

