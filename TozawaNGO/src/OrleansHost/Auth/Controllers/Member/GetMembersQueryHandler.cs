
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using OrleansHost.Attachment.Models.Queries;
using Grains.Helpers;
using System.Buffers;
using System.Collections.Immutable;

namespace Grains.Auth.Controllers
{
    public class GetMembersQueryHandler(IMediator mediator, IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetMembersQuery, TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

        public async Task<TableDataDto<Models.Dtos.Backend.MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            var members = new List<ApplicationUser>();

            // get all item keys for this owner
            var keys = await _factory.GetGrain<IMemberManagerGrain>(SystemTextId.MemberOwnerId).GetAllAsync();

            // fast path for empty owner
            if (keys.Length == 0) return new TableDataDto<MemberDto> { Items = [], TotalItems = 0, ItemPage = 0 };

            // fan out and get all individual items in parallel
            var tasks = ArrayPool<Task<MemberItem>>.Shared.Rent(keys.Length);
            try
            {
                // issue all requests at the same time
                for (var i = 0; i < keys.Length; ++i)
                {
                    tasks[i] = _factory.GetGrain<IMemberGrain>(keys[i]).GetAsync();
                }

                // compose the result as requests complete
                var result = ImmutableArray.CreateBuilder<MemberItem>(tasks.Length);
                for (var i = 0; i < keys.Length; ++i)
                {
                    result.Add(await tasks[i]);
                }

                foreach (var memberItem in result)
                {
                    members.Add(new ApplicationUser
                    {
                        UserId = memberItem.UserId,
                        PartnerId = memberItem.PartnerId,
                        Email = memberItem.Email,
                        Description = memberItem.Description,
                        DescriptionTextId = memberItem.DescriptionTextId,
                        FirstName = memberItem.FirstName,
                        LastName = memberItem.LastName,
                        LastLoginCountry = memberItem.LastLoginCountry,
                        LastLoginCity = memberItem.LastLoginCity,
                        LastLoginState = memberItem.LastLoginState,
                        LastLoginIPAdress = memberItem.LastLoginIPAdress,
                        Adress = memberItem.Adress,
                        UserPasswordHash = memberItem.UserPasswordHash,
                        Roles = memberItem.Roles,
                        LastAttemptLogin = memberItem.LastAttemptLogin,
                        RefreshToken = memberItem.RefreshToken,
                        RefreshTokenExpiryTime = memberItem.RefreshTokenExpiryTime,
                        UserCountry = memberItem.UserCountry,
                        Deleted = memberItem.Deleted,
                        AdminMember = memberItem.AdminMember,
                        LastLogin = memberItem.LastLogin,
                        CreatedBy = memberItem.CreatedBy,
                        CreateDate = memberItem.CreateDate,
                        ModifiedBy = memberItem.ModifiedBy,
                        ModifiedDate = memberItem.ModifiedDate,
                        StationIds = memberItem.StationIds
                    });
                }
                var converted = MemberConverter.Convert(members.Where(x => request.IncludeDeleted || !x.Deleted));

                if (!string.IsNullOrEmpty(request.PageOfEmail))
                {
                    var itemWithEmail = converted.FirstOrDefault(x => x.Email == request.PageOfEmail);
                    if (itemWithEmail != null)
                    {
                        var itemIndex = converted.IndexOf(itemWithEmail);
                        var itemPage = (int)Math.Floor((double)itemIndex / request.PageSize);
                        var pagedItems = converted.Skip(itemPage * request.PageSize).Take(request.PageSize);
                        await SetTranslation(pagedItems);
                        await GetAttachments(pagedItems);
                        return new TableDataDto<MemberDto> { Items = pagedItems, TotalItems = converted.Count, ItemPage = itemPage };
                    }
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    converted = converted.Where(x => x.Email.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    await SetTranslation(converted);
                    await GetAttachments(converted);
                    return new TableDataDto<MemberDto> { Items = converted, TotalItems = converted.Count };
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    converted = converted.Where(Filtered(request.SearchString)).ToList();
                }
                var paged = converted.Skip(request.Page * request.PageSize).Take(request.PageSize);
                await SetTranslation(paged);
                await GetAttachments(paged);
                return new TableDataDto<MemberDto> { Items = paged, TotalItems = converted.Count };
            }
            finally
            {
                ArrayPool<Task<MemberItem>>.Shared.Return(tasks);
            }
        }
        private static Func<MemberDto, bool> Filtered(string searchString) => x => x.Email.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                                                              (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

        private async Task SetTranslation(IEnumerable<MemberDto> members)
        {
            foreach (var member in members)
            {
                if (member.DescriptionTextId != Guid.Empty)
                {
                    var translationItem = await _factory.GetGrain<ITranslationGrain>(member.DescriptionTextId).GetAsync();
                    var translation = new Translation
                    {
                        Id = translationItem.Id,
                        TextId = translationItem.TextId,
                        LanguageText = translationItem.LanguageText,
                        CreateDate = translationItem.CreatedDate,
                        CreatedBy = translationItem.CreatedBy,
                        ModifiedBy = translationItem.ModifiedBy,
                        ModifiedDate = translationItem.ModifiedDate
                    };

                    if (translation != null)
                    {
                        if (translation.LanguageText.TryGetValue(_currentUserService.LanguageId, out string value))
                        {
                            member.Description = value;
                        }
                        else
                        {
                            member.Description = "";
                        }
                    }
                }
            }
        }

        private async Task GetAttachments(IEnumerable<MemberDto> members)
        {
            var request = new GetAttachmentsByOwnerIdsQuery
            {
                OwnerIds = members.Select(x => x.Id).ToList()
            };
            var ownerAttachments = await _mediator.Send(request);
            if (ownerAttachments != null)
            {
                foreach (var ownerAttachment in ownerAttachments)
                {
                    var member = members.First(x => x.Id == ownerAttachment.OwnerId);
                    member.Attachments.AddRange(ownerAttachment.Attachments);
                    if (member.Attachments != null && member.Attachments.Any(x => !string.IsNullOrEmpty(x.MiniatureId) && !string.IsNullOrEmpty(x.Thumbnail)))
                    {
                        member.Thumbnail = member.Attachments.First(x => !string.IsNullOrEmpty(x.MiniatureId) && !string.IsNullOrEmpty(x.Thumbnail)).Thumbnail;
                    }
                }
            }
        }
    }
}