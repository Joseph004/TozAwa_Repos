using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IXliffImportTransaction
    {
        Task Insert(List<Translation> translations, Guid sourceLanguage, Guid targetLanguage, string fileName, Guid fileId);
    }

    public class XliffImportTransaction : IXliffImportTransaction
    {
        private readonly LanguageContext _context;
        public XliffImportTransaction(LanguageContext context) => _context = context;

        public async Task Insert(List<Translation> translations, Guid sourceLanguage, Guid targetLanguage, string fileName, Guid fileId)
        {
            foreach (var translation in translations)
            {
                var translationToUpdate = await _context.Translations.FindAsync(translation.TextId, translation.LanguageId);

                if (translationToUpdate != null)
                {
                    translationToUpdate.Text = translation.Text;
                    translationToUpdate.XliffState = translation.XliffState;
                    translationToUpdate.CreatedAt = DateTime.UtcNow;
                    translationToUpdate.CreatedBy = "xliffImport";
                }
            }

            var logPostforXliffImport = new XliffDistributionFiles
            {
                FileId = fileId,
                FileName = fileName,
                FileState = XliffFileState.Import,
                SourceLanguageId = sourceLanguage,
                TargetLanguageId = targetLanguage,
                NumberOfTranslations = translations.Count,
                CreatedBy = "xliffImport",
                CreatedAt = DateTime.UtcNow
            };

            _context.XliffDistributionFiles.Add(logPostforXliffImport);
            _context.SaveChanges();
        }
    }
}
