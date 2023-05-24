using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.Enum;
using Tozawa.Language.Client.Models.DTOs;
using MudBlazor;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class XliffSearchView : ComponentBase
    {
        [Inject] IXliffHttpClient XliffHttpClient { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        public DateTime? fromDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime? toDate { get; set; } = DateTime.Today.AddDays(1);
        public XliffFileState XliffFileState { get; set; }

        public List<XliffDistributionFile> XliffDistributionFilesLogs { get; set; } = new List<XliffDistributionFile>();
        bool _loading = false;

        private MudTable<XliffDistributionFile> _table;
        protected int _rowsPerPage = 8;
        protected int[] _rowsPerPageOptions = new int[] { 4, 8, 12, 16, 20, 24 };

        private async void GetAllXliffDistributionFilesLogsImp()
        {
            XliffFileState = XliffFileState.Import;
            await _table.ReloadServerData();
        }

        private async void GetAllXliffDistributionFilesLogsExp()
        {
            XliffFileState = XliffFileState.Export;
            await _table.ReloadServerData();
        }

        private async Task<TableData<XliffDistributionFile>> TableReload(TableState state)
        {
            _loading = true;
            var queryParameters = CreateQueryParameters(state);
            var fileLogs = await XliffHttpClient.GetAllXliffDistributionFilesLogsPaged(queryParameters);
            _loading = false;
            return fileLogs;
        }

        private Dictionary<string, string> CreateQueryParameters(TableState state)
        {
            return new Dictionary<string, string> {
                { "StartDate", fromDate.ToString() },
                { "EndDate", toDate.ToString()},
                { "XliffFileState", XliffFileState.ToString() },
                { "page", (state.Page + 1).ToString() },
                { "pageSize", state.PageSize.ToString() }
            };
        }

        public async void DownloadFile(XliffFileState filestate, string fileName)
        {
            byte[] response;
            if (filestate == XliffFileState.Export)
            {
                response = await XliffHttpClient.DownloadExportedFile(fileName);
            }
            else
            {
                response = await XliffHttpClient.DownloadImportedFile(fileName);
            }
            var decoded = System.Text.Encoding.UTF8.GetString(response);
            await JSRuntime.InvokeVoidAsync("download", new
            {
                ByteArray = decoded.Replace("\"", ""),
                FileName = fileName
            });
        }
    }
}