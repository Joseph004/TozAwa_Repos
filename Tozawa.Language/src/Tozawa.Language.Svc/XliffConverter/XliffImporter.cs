using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IXliffImporter
    {
        Task<ImportResult> GetImportResult(string fileName, XDocument document);
    }

    public class XliffImporter : IXliffImporter
    {
        private readonly LanguageContext _context;
        private readonly IImportResultService _importResultService;

        private readonly IXDocumentToXliff _xDocumentToXliff;

        public XliffImporter(LanguageContext context, IImportResultService importResultService, IXDocumentToXliff xDocumentToXliff)
        {
            _context = context;
            _importResultService = importResultService;
            _xDocumentToXliff = xDocumentToXliff;
        }

        public async Task<ImportResult> GetImportResult(string fileName, XDocument document)
        {
            var xliffFile = _xDocumentToXliff.Create(document);
            var targetLangauge = _context.Languages.FirstOrDefault(x => x.ShortName == xliffFile.XliffFileInfo.TargetLanguage);
            if (targetLangauge == null)
            {
                return _importResultService.MissingResult(xliffFile);
            }

            return await _importResultService.ImportResult(fileName, xliffFile, targetLangauge, document);
        }
    }
}