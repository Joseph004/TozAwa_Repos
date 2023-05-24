using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Files;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    public class GetXliffFileByNameQuery : IRequest<byte[]>
    {
        public XliffFileType FileType { get; set; }
        public string Filename { get; set; }

        public GetXliffFileByNameQuery(string filename, XliffFileType fileType)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(GetXliffFileByNameQuery));
            }

            Filename = filename;
            FileType = fileType;
        }
    }

    public enum XliffFileType
    {
        Import,
        Export
    }

    public class GetXliffFileByNameQueryHandler : IRequestHandler<GetXliffFileByNameQuery, byte[]>
    {
        private readonly IAzureFileTasks _azureFileTasks;
        public GetXliffFileByNameQueryHandler(IAzureFileTasks azureFileTasks) => _azureFileTasks = azureFileTasks;

        public async Task<byte[]> Handle(GetXliffFileByNameQuery request, CancellationToken cancellationToken)
        {
            return request.FileType == XliffFileType.Export
                ? await _azureFileTasks.GetExportFile(request.Filename)
                : await _azureFileTasks.GetImportFile(request.Filename);
        }
    }
}
