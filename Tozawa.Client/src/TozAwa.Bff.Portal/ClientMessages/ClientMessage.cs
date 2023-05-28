#nullable disable

using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Helpers;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.ClientMessages;
public static class UpdateMessages
{
    private static ILanguageService _languageService;
    private static AppSettings _appSettings;
    private static ICurrentUserService _currentUserService;
    public static void Configure(ILanguageService languageService, AppSettings appSettings, ICurrentUserService currentUserService)
    {
        _languageService = languageService;
        _appSettings = appSettings;
        _currentUserService = currentUserService;
    }
    public static string EntityDeletedSuccess => _currentUserService?.User?.LanguageId == Guid.Empty ? "Entity deleted successfully" : _languageService.GetSyncBySystemType(SystemTextId.EntityDeletedSuccess, _appSettings.SystemTextGuid);
    public static string Success => _currentUserService?.User?.LanguageId == Guid.Empty ? "Success" : _languageService.GetSyncBySystemType(SystemTextId.Success, _appSettings.SystemTextGuid);
    public static string NotFound => _currentUserService?.User?.LanguageId == Guid.Empty ? "Not found" : _languageService.GetSyncBySystemType(SystemTextId.NotFound, _appSettings.SystemTextGuid);
    public static string Forbidden => _currentUserService?.User?.LanguageId == Guid.Empty ? "Forbidden" : _languageService.GetSyncBySystemType(SystemTextId.Forbidden, _appSettings.SystemTextGuid);
    public static string EntityCreatedSuccess => _currentUserService?.User?.LanguageId == Guid.Empty ? "Entity created successfullt" : _languageService.GetSyncBySystemType(SystemTextId.EntityCreatedSuccess, _appSettings.SystemTextGuid);
    public static string EntityCreatedError => _currentUserService?.User?.LanguageId == Guid.Empty ? "Error when create entity" : _languageService.GetSyncBySystemType(SystemTextId.EntityCreatedError, _appSettings.SystemTextGuid);
    public static string Error => _currentUserService?.User?.LanguageId == Guid.Empty ? "Error" : _languageService.GetSyncBySystemType(SystemTextId.Error, _appSettings.SystemTextGuid);
    public static string Unauthorized => _currentUserService?.User?.LanguageId == Guid.Empty ? "Unauthorized" : _languageService.GetSyncBySystemType(SystemTextId.Unauthorized, _appSettings.SystemTextGuid);
    public static string ImportSuccess => _currentUserService?.User?.LanguageId == Guid.Empty ? "Import success" : _languageService.GetSyncBySystemType(SystemTextId.ImportSuccess, _appSettings.SystemTextGuid);
    public static string ImportError => _currentUserService?.User?.LanguageId == Guid.Empty ? "Error when importing" : _languageService.GetSyncBySystemType(SystemTextId.ImportError, _appSettings.SystemTextGuid);
}