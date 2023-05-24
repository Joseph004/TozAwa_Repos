using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tozawa.Language.Svc.Context;
using Microsoft.EntityFrameworkCore;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IExporter
    {
        Task<XliffDocumentResult> Export(XliffExportParameter xliffExportParameter);
    }

    public class Exporter : IExporter
    {
        private readonly LanguageContext _context;
        public Exporter(LanguageContext context) => _context = context;

        public async Task<XliffDocumentResult> Export(XliffExportParameter xliffExportParameter)
        {
            return await Task.Run(() =>
            {
                var existing = new List<Translation>();
                var nonExisting = new List<Translation>();

                var originalTranslation = GetTranslations(xliffExportParameter.SourceLanguageId, xliffExportParameter.systemTypeId);
                var translatedTranslation = GetTranslations(xliffExportParameter.TargetLangaugeId, xliffExportParameter.systemTypeId);

                var translationIds = originalTranslation.Where(y => y.XliffState == XliffState.Translated && !string.IsNullOrEmpty(y.Text)).Select(x => x.TextId).ToList();
                var sourceLangauge = _context.Languages.First(x => x.Id == xliffExportParameter.SourceLanguageId);
                var targetLangauge = _context.Languages.First(x => x.Id == xliffExportParameter.TargetLangaugeId);
                var translateableTexts = new List<TranslateableText>();

                foreach (var translationId in translationIds)
                {
                    AddTranslationToXliffLists(translatedTranslation, translationId, existing, nonExisting, originalTranslation, targetLangauge, translateableTexts);
                }

                var fileId = Guid.NewGuid();

                var converted = XliffExporter.Export(translateableTexts, new CultureInfo(sourceLangauge.ShortName), new CultureInfo(targetLangauge.ShortName), fileId);
                return CreateXliffDocumentResult(xliffExportParameter, converted, existing, nonExisting, fileId, translateableTexts);
            });
        }

        private List<Translation> GetTranslations(Guid languageId, Guid systemTypeId)
        {
            return _context.Translations
                .Include(x => x.SystemType)
                .Where(x => x.LanguageId == languageId && x.SystemTypeId == systemTypeId && x.Deleted == false)
                .ToList();
        }

        private static XliffDocumentResult CreateXliffDocumentResult(XliffExportParameter xliffExportParameter, XDocument converted, List<Translation> translationsThatExistsAndNeedsToBeSetToXliffStateAwayOnTranslation, List<Translation> translationsThatNotExistsAndNeedsToBeInsertedEmptyWithXliffStateAwayOnTranslation, Guid fileId, List<TranslateableText> translateableTexts)
        {
            return new XliffDocumentResult
            {
                XDocument = converted,
                Existing = translationsThatExistsAndNeedsToBeSetToXliffStateAwayOnTranslation,
                NonExisting = translationsThatNotExistsAndNeedsToBeInsertedEmptyWithXliffStateAwayOnTranslation,
                FileIdForLog = fileId,
                SourceLanguageForLog = xliffExportParameter.SourceLanguageId,
                TargetLanguageForLog = xliffExportParameter.TargetLangaugeId,
                TranslatableTexts = translateableTexts
            };
        }

        private void AddTranslationToXliffLists(List<Translation> translatedTranslationTexts, Guid translationId,
            List<Translation> existing,
            List<Translation> nonExisting,
            List<Translation> originalTranslations, Languagemd targetLangauge, List<TranslateableText> translateableTexts)
        {
            var translated = translatedTranslationTexts.FirstOrDefault(x => x.TextId == translationId);
            if (translated != null)
            {
                if (translated.XliffState != XliffState.NeedsTranslation)
                {
                    return;
                }

                existing.Add(translated);
            }
            else
            {
                nonExisting.Add(CreateTranslation(originalTranslations,
                    targetLangauge, translationId)
                );
            }

            AddText(originalTranslations, translateableTexts, translationId, translated);
        }

        private void AddText(List<Translation> originalTranslationTexts, List<TranslateableText> translateableTexts, Guid translationId, Translation translated)
        {
            var aa = originalTranslationTexts.FirstOrDefault(x => x.TextId == translationId);
            translateableTexts.Add(new TranslateableText
            {
                Id = translationId.ToString(),
                SystemTypeName = originalTranslationTexts.First(x => x.TextId == translationId).SystemType.Description,
                Context = String.Empty,
                Source = originalTranslationTexts.First(x => x.TextId == translationId).Text,
                Target = translated != null ? translated.Text : "",
                TranslationState = translated?.XliffState ?? XliffState.NeedsTranslation,
            });
        }

        private Translation CreateTranslation(List<Translation> originalTranslationTexts, Languagemd targetLangauge, Guid translationId)
        {
            return new Translation
            {
                TextId = translationId,
                Text = string.Empty,
                Deleted = false,
                CreatedBy = "xliffExport",
                CreatedAt = DateTime.Now,
                LanguageId = targetLangauge.Id,
                XliffState = XliffState.AwayOnTranslation,
                SystemTypeId = originalTranslationTexts.First(x => x.TextId == translationId).SystemTypeId
            };
        }
    }
}