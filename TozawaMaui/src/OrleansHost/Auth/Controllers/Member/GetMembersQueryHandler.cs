
using MediatR;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos.Backend;
using System.Buffers;
using System.Collections.Immutable;
using OrleansHost.Auth.Models.Queries;
using Grains.Auth.Models.Dtos;

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

            ImmutableArray<Guid> keys = [];
            if (request.ByPassOrganization)
            {
                // get all item keys for this owner
                keys = await _factory.GetGrain<IMemberManagerGrain>(_currentUserService.User.Organizations.First(x => x.PrimaryOrganization).Id).GetAllAsync();

                // fast path for empty owner
                if (keys.Length == 0) return new TableDataDto<MemberDto> { Items = [], TotalItems = 0, ItemPage = 0 };
            }
            else
            {
                var workingOrganization = await _factory.GetGrain<IOrganizationGrain>(_currentUserService.User.WorkingOrganizationId).GetAsync();
                foreach (var item in workingOrganization.UserIds)
                {
                    keys = keys.Add(item);
                }
            }
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
                    var organizations = new List<CurrentUserOrganizationDto>();
                    foreach (var orgItem in memberItem.OrganizationIds)
                    {
                        var org = (await _mediator.Send(new GetOrganizationQuery { Id = orgItem }, cancellationToken)) ?? new();
                        organizations.Add(new CurrentUserOrganizationDto
                        {
                            Id = org.Id,
                            Addresses = org.Addresses,
                            Features = org.Features,
                            Name = org.Name,
                            Active = true,
                            PrimaryOrganization = org.Id == memberItem.OwnerKey
                        });
                    }
                    var primaryOrganization = organizations.First(x => x.PrimaryOrganization);
                    var roleDtos = ((await _mediator.Send(new GetRolesQuery(memberItem.UserId), cancellationToken)) ?? []).Where(x => x.OrganizationId == primaryOrganization.Id);
                    var addressesDtos = await _mediator.Send(new GetAddressesQuery(memberItem.UserId), cancellationToken);
                    var functions = roleDtos.SelectMany(x => x.Functions).Select(y => new CurrentUserFunctionDto
                    {
                        FunctionType = y.FunctionType
                    }).Distinct().ToList();
                    var member = MemberConverter.Convert(memberItem, organizations, addressesDtos.ToList(), roleDtos.ToList(), functions);
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
                    converted = [.. converted.Where(Filtered(request.SearchString))];
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