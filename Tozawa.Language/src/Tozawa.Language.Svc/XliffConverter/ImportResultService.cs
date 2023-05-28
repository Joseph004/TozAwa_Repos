using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Files;

using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IImportResultService
    {
        Task<ImportResult> ImportResult(string fileName, XliffFile xliffFile, Languagemd targetLangauge, XDocument document);
        ImportResult MissingResult(XliffFile xliffFile);
    }

    public class ImportResultService : IImportResultService
    {
        private readonly LanguageContext _context;
        private readonly IAzureFileTasks _azureFileTasks;
        private readonly IXliffImportTransaction _fileTransaction;

        public ImportResultService(LanguageContext context, IAzureFileTasks azureFileTasks, IXliffImportTransaction fileTransaction)
        {
            _context = context;
            _azureFileTasks = azureFileTasks;
            _fileTransaction = fileTransaction;
        }

        public async Task<ImportResult> ImportResult(string fileName, XliffFile xliffFile, Languagemd targetLangauge, XDocument document)
        {
            var result = new ImportResult { Ok = true };
            var translations = xliffFile.TranslateableTexts.Select(translateableText => CreateTranslation(targetLangauge.Id, translateableText)).ToList();

            var translated = translations.Where(x => x.XliffState == XliffState.Translated && x.Text.Trim() != string.Empty).ToList();

            var sourceLangauge = _context.Languages.FirstOrDefault(x => x.ShortName == xliffFile.XliffFileInfo.SourceLanguage);
            if (sourceLangauge == null)
            {
                throw new ArgumentNullException(nameof(xliffFile.XliffFileInfo.SourceLanguage));
            }

            await _fileTransaction.Insert(translated, sourceLangauge.Id, targetLangauge.Id, fileName, Guid.Parse(xliffFile.XliffFileInfo.FileId));

            await _azureFileTasks.SaveImportFile(document, fileName);
            if (!result.Ok)
            {
                throw new ArgumentNullException(result.Reason);
            }
            return result;
        }

        public ImportResult MissingResult(XliffFile xliffFile)
        {
            var result = new ImportResult
            {
                Ok = false,
                Reason = $"TargetLangauge {xliffFile.XliffFileInfo.TargetLanguage} is missing or not recognized"
            };
            return result;
        }

        private static Translation CreateTranslation(Guid targetLanguageId, TranslateableText translateableText)
        {
            return new Translation
            {
                XliffState = XliffState.Translated,
                LanguageId = targetLanguageId,
                Text = translateableText.Target,
                TextId = Guid.Parse(translateableText.Id)
            };
        }
    }
}