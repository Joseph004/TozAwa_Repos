
using Microsoft.Extensions.Caching.Memory;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Client.Portal.HttpClients;

namespace Tozawa.Bff.Portal.Services
{
    public interface IMemberService : IEntityService<Models.Dtos.Backend.MemberDto>
    {
    }
    public class MemberService : EntityService<Models.Dtos.Backend.MemberDto>, IMemberService
    {
        private readonly ILanguageText _languageText;
        private readonly ITozAwaAuthHttpClient _tozAwaAuthHttpClient;
        public MemberService(ILanguageText languageText, IMemoryCache memoryCache, ITozAwaAuthHttpClient tozAwaAuthHttpClient, ICurrentUserService currentUserService, ILogger<MemberService> logger) : base(memoryCache, currentUserService, logger)
        {
            _languageText = languageText;
            _tozAwaAuthHttpClient = tozAwaAuthHttpClient;
        }

        protected override async Task<List<Models.Dtos.Backend.MemberDto>> ApiCall()
            => await _tozAwaAuthHttpClient.Get<List<Models.Dtos.Backend.MemberDto>>("member?includeDeleted=true");
    }
}