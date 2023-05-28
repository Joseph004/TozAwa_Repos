using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tozawa.Language.Svc.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Tozawa.Language.Svc.Files
{
    public interface IAzureFileTasks
    {
        Task SaveExportFile(XDocument xDocumentFile, string filename);
        Task SaveImportFile(XDocument xDocumentFile, string filename);
        Task<byte[]> GetExportFile(string filename);
        Task<byte[]> GetImportFile(string filename);
    }

    public class AzureFileTasks : IAzureFileTasks
    {
        private readonly AppSettings _appSettings;
        protected CloudBlobClient Client;
        protected CloudBlobContainer ImportContainer;
        protected CloudBlobContainer ExportContainer;

        public AzureFileTasks(AppSettings appSettings)
        {
            _appSettings = appSettings;
            SetupClient();
            SetupContainers();
        }

        private void SetupContainers()
        {
            if (Client == null)
            {
                SetupClient();
            }
            ImportContainer = Client.GetContainerReference(_appSettings.AzureImportFilesDirectory);
            var importContainerTask = ImportContainer.CreateIfNotExistsAsync();
            importContainerTask.Wait();

            ExportContainer = Client.GetContainerReference(_appSettings.AzureExportFilesDirectory);
            var exportContainerTask = ExportContainer.CreateIfNotExistsAsync();
            exportContainerTask.Wait();
        }

        private void SetupClient()
        {
            var storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionstring);
            Client = storageAccount.CreateCloudBlobClient();
        }

        public async Task SaveExportFile(XDocument xDocumentFile, string filename)
        {
            await UpLoadToAzure(xDocumentFile, filename, ExportContainer);
        }

        public async Task SaveImportFile(XDocument xDocumentFile, string filename)
        {
            await UpLoadToAzure(xDocumentFile, filename, ImportContainer);
        }

        private async Task UpLoadToAzure(XDocument xDocumentFile, string filename, CloudBlobContainer container)
        {
            var stream = ConvertxDocumentToMemoryStream(xDocumentFile);
            var blob = container.GetBlockBlobReference(filename);
            await blob.UploadFromStreamAsync(stream);
        }

        public async Task<byte[]> GetExportFile(string filename)
        {
            return (await GetFileStreamFromAzure(filename, ExportContainer)).ToArray();
        }

        public async Task<byte[]> GetImportFile(string filename)
        {
            return (await GetFileStreamFromAzure(filename, ImportContainer)).ToArray();
        }

        private async Task<MemoryStream> GetFileStreamFromAzure(string filename, CloudBlobContainer container)
        {
            var blob = container.GetBlockBlobReference(filename);
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                return stream;
            }
        }

        private static MemoryStream ConvertxDocumentToMemoryStream(XDocument cloudxDocFile)
        {
            var stream = new MemoryStream();
            cloudxDocFile.Save(stream);
            stream.Position = 0;
            return stream;
        }
    }
}