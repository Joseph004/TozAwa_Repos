using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Microsoft.EntityFrameworkCore;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IXliffExportTransaction
    {
        Task UpdateAll(XliffDocumentResult result, string fileName);
    }

    public class XliffExportTransaction : IXliffExportTransaction
    {
        private readonly LanguageContext _context;

        public XliffExportTransaction(LanguageContext context)
        {
            _context = context;
        }

        public async Task UpdateAll(XliffDocumentResult result, string fileName)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>

            {
                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        InsertExisting(result.Existing);
                        InsertEmpty(result.NonExisting);

                        _context.XliffDistributionFiles.Add(CreateLogs(result, fileName).Result);
                        _context.SaveChanges();

                        dbContextTransaction.Commit();
                        await Task.FromResult(0);
                    }
                    catch (Exception exc)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Error with transacation, operation rollbacked. Exception : " + exc);
                    }
                }
            });
        }

        private static async Task<XliffDistributionFiles> CreateLogs(XliffDocumentResult result, string fileName)
        {
            var requestedDeliveryDate = DateTime.Now.AddDays(30);
            return await Task.Run(() =>
            {
                var sourceTranslations = result.TranslatableTexts.Select(x => x.Source).ToList();
                var xliffDistributionFiles = new XliffDistributionFiles
                {
                    FileId = result.FileIdForLog,
                    FileName = fileName,
                    FileState = XliffFileState.Export,
                    SourceLanguageId = result.SourceLanguageForLog,
                    TargetLanguageId = result.TargetLanguageForLog,
                    NumberOfTranslations = result.TranslatableTexts.Count,
                    NumberOfWordsSentInSourceLanguage = CountNumberOfWords(sourceTranslations),
                    RequestedDeliveryDate = requestedDeliveryDate,
                    CreatedBy = "xliffExport",
                    CreatedAt = DateTime.Now
                };
                return xliffDistributionFiles;
            });
        }

        private static int CountNumberOfWords(IEnumerable<string> listOfstrings)
        {
            var counter = 0;
            foreach (var oneString in listOfstrings)
            {
                var words = oneString.Trim().Split(' ');
                counter += words.Length;
            }

            return counter;
        }

        private void InsertExisting(IEnumerable<Translation> existing)
        {
            foreach (var translation in existing)
            {
                var translationToUpdate = _context.Translations.Find(translation.TextId, translation.LanguageId);
                translationToUpdate.XliffState = XliffState.AwayOnTranslation;
            }
        }

        private void InsertEmpty(IEnumerable<Translation> nonExisting)
        {
            foreach (var emptyTranslation in nonExisting)
            {
                _context.Translations.Add(emptyTranslation);
            }
        }
    }
}