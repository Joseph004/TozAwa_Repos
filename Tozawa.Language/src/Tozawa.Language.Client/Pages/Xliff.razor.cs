using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net.Http.Headers;
using Tozawa.Language.Client.HttpClients;

namespace Tozawa.Language.Client.Pages
{
    public partial class Xliff : ComponentBase
    {
        [Inject] IXliffHttpClient XliffHttpClient { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        IReadOnlyList<IBrowserFile> SelectedFiles;
        private bool InputFile = true;

        public async void UploadFile()
        {
            using var content = new MultipartFormDataContent();
            foreach (var file in SelectedFiles)
            {
                var maxSize = file.Size;
                var streamContent = new StreamContent(file.OpenReadStream(maxSize));
                var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                content.Add(fileContent, "file", file.Name);

                await XliffHttpClient.UploadFile(content);
                Snackbar.Add("File imported", Severity.Success);
            }
            ReRenderInputFile();
        }

        private void ReRenderInputFile()
        {
            InputFile = false;
            StateHasChanged();
            InputFile = true;
            StateHasChanged();
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            SelectedFiles = e.GetMultipleFiles();
            StateHasChanged();
        }
    }
}
