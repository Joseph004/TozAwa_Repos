using Microsoft.Extensions.Caching.Memory;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Helpers;

namespace Tozawa.Bff.Portal.Services
{
#nullable enable
    public class EntityService<TType> where TType : Models.Dtos.Backend.IdCodeEntity
    {
        protected readonly string _loggingEntityName;
        protected readonly string _cacheConstant;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger _logger;
        public EntityService(IMemoryCache memoryCache, ICurrentUserService currentUserService, ILogger logger)
        {
            _memoryCache = memoryCache;
            _currentUserService = currentUserService;
            _logger = logger;
            _loggingEntityName = nameof(TType);
            _cacheConstant = nameof(TType);
        }
        public Guid _organizationId => _currentUserService.User.OrganizationId;
        public Guid _languageId => _currentUserService.LanguageId == Guid.Empty ? Guid.Parse("60e127fb-f4fc-45a5-9d3b-32934d27edca") : _currentUserService.LanguageId;
        public Guid _oid => _currentUserService.User.Oid;
        protected virtual async Task<List<TType>> ApiCall()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }
        public async virtual Task SortItems()
        {
            var organizationId = _currentUserService.User.OrganizationId;
            var items = await GetItems();
            var sorted = items.OrderByDescending(x => x.CreatedDate).ToList();
            _memoryCache.Set(GetCacheKey(organizationId), sorted);
        }
        public async Task<TType?> GetItem(Guid id)
        {
            var items = await GetItems();
            return items.Find(x => x.Id == id);
        }
        public async Task<TType?> GetItem(string code)
        {
            var items = await GetItems();
            return items.Find(x => x.Code == code);
        }
        public async Task<List<TType>> GetItems()
        {
            if (_memoryCache.TryGetValue(GetCacheKey(_organizationId), out List<TType> cacheEntry))
            {
                return cacheEntry;
            }
            cacheEntry = (await GetFromApi(_organizationId, _oid, _languageId)).OrderByAlphaNumeric(x => x.Code).ToList();
            _memoryCache.Set(GetCacheKey(_organizationId), cacheEntry);
            return cacheEntry;
        }
        private async Task<List<TType>> GetFromApi(Guid organizationId, Guid oid, Guid languageId)
        {
            try
            {
                return await ApiCall();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve {entityName} due to exception organizationid: {organizationId} user {oid} language {languageId} exception {exception}", _loggingEntityName, organizationId, oid, languageId, ex.Message);
                throw new Exception($"Unable to retrieve {_loggingEntityName} due to server error");
            }
        }
        protected string GetCacheKey(Guid organizationId) => $"{_cacheConstant}.{organizationId}";
        public void ClearCache() => _memoryCache.Remove(GetCacheKey(_currentUserService.User.OrganizationId));
    }
}