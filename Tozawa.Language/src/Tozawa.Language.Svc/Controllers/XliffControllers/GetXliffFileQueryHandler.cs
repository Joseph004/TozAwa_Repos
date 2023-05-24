using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Files;
using Tozawa.Language.Svc.XliffConverter;
using MediatR;
using Tozawa.Language.Svc.Models;
using System;
using FluentValidation;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    public class GetXliffFileQuery : IRequest<byte[]>
    {
        public Guid SourceLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
        public string FileName { get; set; }

        public GetXliffFileQuery(Guid sourceLanguageId, Guid targetLanguageId, Guid systemTypeId, string fileName)
        {
            SourceLanguageId = sourceLanguageId;
            TargetLanguageId = targetLanguageId;
            SystemTypeId = systemTypeId;
            FileName = fileName;
        }
    }

    public class GetXliffFileQueryValidator : AbstractValidator<GetXliffFileQuery>
    {
        public GetXliffFileQueryValidator()
        {
            RuleFor(x => x.SourceLanguageId).NotEmpty();
            RuleFor(x => x.TargetLanguageId).NotEmpty();
            RuleFor(x => x.SystemTypeId).NotEmpty();
            RuleFor(x => x.FileName).NotEmpty();
        }
    }

    public class GetXliffFileQueryHandler : IRequestHandler<GetXliffFileQuery, byte[]>
    {
        private readonly IXliffExportTransaction _xliffExportTransaction;
        private readonly IExporter _exporter;
        private readonly IAzureFileTasks _azureFileTasks;

        public GetXliffFileQueryHandler(IXliffExportTransaction xliffExportTransaction, IExporter exporter, IAzureFileTasks azureFileTasks)
        {
            _xliffExportTransaction = xliffExportTransaction;
            _exporter = exporter;
            _azureFileTasks = azureFileTasks;
        }

        public async Task<byte[]> Handle(GetXliffFileQuery request, CancellationToken cancellationToken)
        {
            var xliffExportParameter = CreateExportParameter(request);
            var xliffResult = await _exporter.Export(xliffExportParameter);

            await _xliffExportTransaction.UpdateAll(xliffResult, request.FileName);
            await _azureFileTasks.SaveExportFile(xliffResult.XDocument, request.FileName);

            return CreateStream(xliffResult).ToArray();
        }

        private static MemoryStream CreateStream(XliffDocumentResult xliffResult)
        {
            var stream = new MemoryStream();
            xliffResult.XDocument.Save(stream);
            stream.Position = 0;
            return stream;
        }

        private static XliffExportParameter CreateExportParameter(GetXliffFileQuery request)
        {
            var xliffExportParameter = new XliffExportParameter
            {
                SourceLanguageId = request.SourceLanguageId,
                TargetLangaugeId = request.TargetLanguageId,
                systemTypeId = request.SystemTypeId
            };
            return xliffExportParameter;
        }
    }
}
