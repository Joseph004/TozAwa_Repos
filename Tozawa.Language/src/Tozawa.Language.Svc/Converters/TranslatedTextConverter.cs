using System.Collections.Generic;
using System.Linq;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.Converters
{
    public interface ITranslatedTextConverter
    {
        IEnumerable<TranslatedTextDto> Convert(IEnumerable<Translation> sourceTranslations, IEnumerable<Translation> translated);
    }

    public class TranslatedTextConverter : ITranslatedTextConverter
    {
        public IEnumerable<TranslatedTextDto> Convert(IEnumerable<Translation> sourceTranslations, IEnumerable<Translation> translated)
        {
            var translatedTexts = GetTranslatedTextsFromSource(sourceTranslations);
            SetTranslated(translated, translatedTexts);
            return translatedTexts;
        }

        private static List<TranslatedTextDto> GetTranslatedTextsFromSource(IEnumerable<Translation> sourceTranslations)
        {
            var translatedTexts = new List<TranslatedTextDto>();
            foreach (var sourceTranslation in sourceTranslations)
            {
                translatedTexts.Add(new TranslatedTextDto
                {
                    Id = sourceTranslation.TextId,
                    Source = sourceTranslation.Text,
                    XliffState = XliffState.NeedsTranslation,
                    Deleted = sourceTranslation.Deleted,
                    SystemTypeId = sourceTranslation.SystemTypeId,
                });
            }
            return translatedTexts;
        }

        private static void SetTranslated(IEnumerable<Translation> translated, List<TranslatedTextDto> translatedTexts)
        {
            foreach (var translation in translated)
            {
                var firstOrDefault = translatedTexts.FirstOrDefault(x => x.Id == translation.TextId && x.SystemTypeId == translation.SystemTypeId);
                if (firstOrDefault != null)
                {
                    firstOrDefault.Target = translation.Text;
                    firstOrDefault.XliffState = translation.XliffState;
                }
            }
        }
    }
}