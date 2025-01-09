
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using Grains.Helpers;
using System.Buffers;
using System.Collections.Immutable;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers
{
    public class GetMembersQueryHandler(IMediator mediator, IGrainFactory factory, Grains.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetMembersQuery, TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly Grains.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

        public async Task<TableDataDto<Models.Dtos.Backend.MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsAdminRequest && _currentUserService.User.Admin)
            {
                return new TableDataDto<MemberDto> { Items = [], TotalItems = 0, ItemPage = 0 };
            }

            // get all item keys for this owner
            var keys = await _factory.GetGrain<IMemberManagerGrain>(_currentUserService.User.Organizations.First(x => x.PrimaryOrganization).Id).GetAllAsync();

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
                List<MemberDto> converted = [];
                foreach (var memberItem in result)
                {
                    if (!request.IncludeDeleted && memberItem.Deleted) continue;

                    if (!request.IsAdminRequest && (memberItem.LandLords == null || !memberItem.LandLords.Contains(_currentUserService.User.Id)))
                    {
                        continue;
                    }
                    if ((_currentUserService.User.Roles == null || !_currentUserService.User.Roles.Any(x => x.Role == Models.Dtos.Role.President)) && _currentUserService.User.Id == memberItem.UserId)
                    {
                        continue;
                    }
                    var roleDtos = await _mediator.Send(new GetRolesQuery(memberItem.UserId));
                    var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId));
                    var member = MemberConverter.Convert(new ApplicationUser
                    {
                        UserId = memberItem.UserId,
                        Email = memberItem.Email,
                        Description = memberItem.Description,
                        DescriptionTextId = memberItem.DescriptionTextId,
                        Comment = memberItem.Comment,
                        CommentTextId = memberItem.CommentTextId,
                        FirstName = memberItem.FirstName,
                        LastName = memberItem.LastName,
                        LastLoginCountry = memberItem.LastLoginCountry,
                        LastLoginCity = memberItem.LastLoginCity,
                        LastLoginState = memberItem.LastLoginState,
                        LastLoginIPAdress = memberItem.LastLoginIPAdress,
                        Roles = roleDtos.Select(y => new UserRole
                        {
                            UserId = memberItem.UserId,
                            RoleId = y.Id,
                            OrganizationId = y.OrganizationId,
                            Role = new Role
                            {
                                Id = y.Id,
                                RoleEnum = (RoleEnum)y.Role,
                                Functions = y.Functions.Select(f => new Function
                                {
                                    Functiontype = f.FunctionType,
                                    RoleId = f.RoleId,
                                    Role = new Role
                                    {
                                        OrganizationId = y.OrganizationId,
                                        Functions = y.Functions.Select(t => new Function
                                        {
                                            Functiontype = t.FunctionType
                                        }).ToList()
                                    }
                                }).ToList()
                            }
                        }).ToList(),
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
                    }, addressesDtos.ToList());
                    member.AttachmentsCount = memberItem.AttachmentsCount;
                    member.Timestamp = memberItem.Timestamp;
                    await SetTranslation(member);
                    converted.Add(member);
                }

                if (!string.IsNullOrEmpty(request.PageOfEmail))
                {
                    var itemWithEmail = converted.FirstOrDefault(x => x.Email == request.PageOfEmail);
                    if (itemWithEmail != null)
                    {
                        var itemIndex = converted.IndexOf(itemWithEmail);
                        var itemPage = (int)Math.Floor((double)itemIndex / request.PageSize);
                        var pagedItems = converted.Skip(itemPage * request.PageSize).Take(request.PageSize);
                        return new TableDataDto<MemberDto> { Items = pagedItems, TotalItems = converted.Count, ItemPage = itemPage };
                    }
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    converted = converted.Where(x => x.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    return new TableDataDto<MemberDto> { Items = converted, TotalItems = converted.Count };
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    converted = converted.Where(Filtered(request.SearchString)).ToList();
                }
                var paged = converted.Skip(request.Page * request.PageSize).Take(request.PageSize);
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
                                                                                (!string.IsNullOrEmpty(x.Comment) && x.Comment.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
        private async Task SetTranslation(MemberDto member)
        {
            if (member.DescriptionTextId != Guid.Empty)
            {
                var translationItem = await _factory.GetGrain<ITranslationGrain>(member.DescriptionTextId).GetAsync();
                var translation = translationItem != null ? new Translation
                {
                    Id = translationItem.Id,
                    TextId = translationItem.TextId,
                    LanguageText = translationItem.LanguageText,
                    CreateDate = translationItem.CreatedDate,
                    CreatedBy = translationItem.CreatedBy,
                    ModifiedBy = translationItem.ModifiedBy,
                    ModifiedDate = translationItem.ModifiedDate
                } : new Translation();

                if (translation != null && translation.TextId != Guid.Empty)
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

            if (member.CommentTextId != Guid.Empty)
            {
                var translationItem = await _factory.GetGrain<ITranslationGrain>(member.CommentTextId).GetAsync();
                var translation = translationItem != null ? new Translation
                {
                    Id = translationItem.Id,
                    TextId = translationItem.TextId,
                    LanguageText = translationItem.LanguageText,
                    CreateDate = translationItem.CreatedDate,
                    CreatedBy = translationItem.CreatedBy,
                    ModifiedBy = translationItem.ModifiedBy,
                    ModifiedDate = translationItem.ModifiedDate
                } : new Translation();

                if (translation != null && translation.TextId != Guid.Empty)
                {
                    if (translation.LanguageText.TryGetValue(_currentUserService.LanguageId, out string value))
                    {
                        member.Comment = value;
                    }
                    else
                    {
                        member.Comment = "";
                    }
                }
            }
        }
    }
}