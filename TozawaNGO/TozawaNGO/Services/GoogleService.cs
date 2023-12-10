using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using TozawaNGO.Configurations;
using static Google.Apis.Drive.v3.DriveService;

namespace TozawaNGO.Services
{
    public interface IGoogleService
    {
        Task<string> UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription);
        Task DeleteFile(string fileId);
        Task<IEnumerable<Google.Apis.Drive.v3.Data.File>> GetFolderFiles(string folder);
        Task<Google.Apis.Drive.v3.Data.File> GetFileInFolder(string folder, string id);
        Task<Stream> StreamFromGoogleFileByFolder(string folder, string id);
        Task<Stream> StreamFromGoogleFileByFileId(string id);
    }
    public class GoogleService : IGoogleService
    {
        private readonly ILogger<GoogleService> _logger;
        private readonly AppSettings _appSettings;
        public GoogleService(AppSettings appSettings, ILogger<GoogleService> logger)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<DriveService> GetService()
        {
            var service = new DriveService();
            string[] scopes = new string[] { Scope.Drive };
            try
            {
                var dynamicObject = JsonConvert.SerializeObject(_appSettings.GoogleDrive);

                var credential = GoogleCredential.FromJson(dynamicObject)
                .CreateScoped(DriveService.ScopeConstants.Drive);

                service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Toz'Awa"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting google drive service");
            }

            return await Task.FromResult(service);
        }
        private async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFolderList(string folder)
        {
            var service = await GetService();
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{folder}'";
            listRequest.Fields = "nextPageToken, files(id, name)";

            return listRequest.Execute()
            .Files;
        }
        private async Task<Google.Apis.Drive.v3.Data.File> CreateParentFoler(string parentFolder)
        {
            var service = await GetService();

            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = parentFolder;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();

            return file;
        }
        private async Task<Google.Apis.Drive.v3.Data.File> CreateFoler(string folder, string parentFolderId)
        {
            var service = await GetService();

            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folder;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            driveFolder.Parents = new string[] { $"{parentFolderId}" };
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();

            return file;
        }
        public async Task<string> GetOrCreateFolder(string folder)
        {
            var files = await GetFolderList(folder);
            if (files != null && files.Count > 0)
            {
                if (files.Any(x => x.Name.Equals(folder)))
                {
                    return files.FirstOrDefault(x => x.Name.Equals(folder)).Id;
                }
                else
                {
                    var listFile = await GetFolderList("TozAwaSrc");
                    var parentFile = new Google.Apis.Drive.v3.Data.File();
                    if (files != null && files.Count > 0)
                    {
                        parentFile = files.FirstOrDefault(x => x.Name.Equals(folder));
                    }
                    else
                    {
                        parentFile = await CreateParentFoler("TozAwaSrc");
                    }

                    var file = await CreateFoler(folder, parentFile.Id);
                    return file.Id;
                }
            }
            else
            {
                var listFile = await GetFolderList("TozAwaSrc");
                var parentFile = new Google.Apis.Drive.v3.Data.File();
                if (listFile != null && listFile.Count > 0)
                {
                    parentFile = listFile.FirstOrDefault(x => x.Name.Equals("TozAwaSrc"));
                }
                else
                {
                    parentFile = await CreateParentFoler("TozAwaSrc");
                }

                var file = await CreateFoler(folder, parentFile.Id);
                return file.Id;
            }
        }
        public async Task<string> UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {
            DriveService service = await GetService();

            var folderId = await GetOrCreateFolder(folder);

            var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.Parents = new string[] { folderId };


            var request = service.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";

            var response = request.Upload();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw response.Exception;

            return request.ResponseBody.Id;
        }
        public async Task DeleteFile(string fileId)
        {
            var service = await GetService();
            var command = service.Files.Delete(fileId);
            var result = command.Execute();
        }
        public async Task<IEnumerable<Google.Apis.Drive.v3.Data.File>> GetFolderFiles(string folder)
        {
            var service = await GetService();
            var result = new List<Google.Apis.Drive.v3.Data.File>();

            try
            {
                var folderId = (await GetFolderList(folder)).FirstOrDefault(x => x.Name == folder).Id;
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 50;
                listRequest.Q = "'" + folderId + "' in parents and trashed=false";
                listRequest.Fields = "nextPageToken, files(id, name, size, mimeType)";

                result = listRequest.Execute()
                .Files.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting list of files");
            }

            return result;
        }
        public async Task<Google.Apis.Drive.v3.Data.File> GetFileInFolder(string folder, string id)
        {
            var files = await GetFolderFiles(folder);
            var result = new Google.Apis.Drive.v3.Data.File();
            if (files != null && files.Any())
            {
                result = files.FirstOrDefault(x => x.Id == id);
            }
            return result;
        }
        public async Task<Stream> StreamFromGoogleFileByFolder(string folder, string id)
        {
            var service = await GetService();
            var stream = new System.IO.MemoryStream();

            try
            {
                FilesResource.GetRequest request = service.Files.Get(id);

                request.DownloadWithStatus(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file");
            }

            return stream;
        }
        public async Task<Google.Apis.Drive.v3.Data.File> GetFileById(string id)
        {
            var service = await GetService();
            var result = new Google.Apis.Drive.v3.Data.File();

            try
            {
                FilesResource.GetRequest request = service.Files.Get(id);

                result = request.Execute();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file");
            }

            return result;
        }
        public async Task<Stream> StreamFromGoogleFileByFileId(string id)
        {
            var service = await GetService();
            var stream = new System.IO.MemoryStream();

            try
            {
                FilesResource.GetRequest request = service.Files.Get(id);

                request.DownloadWithStatus(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file");
            }

            return stream;
        }
    }
}