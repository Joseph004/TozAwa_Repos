using System;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Services;

public interface ITranslationService
{
    Task<Dictionary<Guid, string>> GetSystemTexts(Guid languageId, Guid systemTypeId);
    List<ActiveLanguageDto> GetActiveLanguages();
    Task<string> GetHttpStatusText(HttpStatusCode? code);
}
