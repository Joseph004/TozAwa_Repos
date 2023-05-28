using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.XliffConverter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    public class ImportXliffFilesCommand : IRequest
    {
        public List<IFormFile> Files { get; set; }
    }

    public class ImportXliffFilesCommandValidator : AbstractValidator<ImportXliffFilesCommand>
    {
        public ImportXliffFilesCommandValidator()
        {
            RuleFor(x => x.Files).NotEmpty();
        }
    }

    public class ImportXliffFilesCommandHandler : IRequestHandler<ImportXliffFilesCommand>
    {
        private readonly IXliffConverter _xliffConverter;
        private readonly IXliffImporter _xliffImporter;

        public ImportXliffFilesCommandHandler(IXliffConverter xliffConverter, IXliffImporter xliffImporter)
        {
            _xliffConverter = xliffConverter;
            _xliffImporter = xliffImporter;
        }

        public async Task<Unit> Handle(ImportXliffFilesCommand request, CancellationToken cancellationToken)
        {
            foreach (var requestFile in request.Files)
            {
                using (var stream = new MemoryStream())
                {
                    await requestFile.CopyToAsync(stream, CancellationToken.None);
                    var bytes = stream.ToArray();
                    var converted = _xliffConverter.StreamToXDocument(bytes);
                    await _xliffImporter.GetImportResult(requestFile.FileName, converted);
                }
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}
