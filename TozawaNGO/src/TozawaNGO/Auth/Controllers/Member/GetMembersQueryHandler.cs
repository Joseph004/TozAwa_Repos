
using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Context;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Converters;
using TozawaNGO.Auth.Models.Dtos.Backend;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Services;

namespace TozawaNGO.Auth.Controllers
{
    public class GetMembersQueryHandler(TozawangoDbContext context, IMediator mediator, IGoogleService googleService, TozawaNGO.Auth.Services.ICurrentUserService currentUserService) : IRequestHandler<GetMembersQuery, TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
        private readonly TozawangoDbContext _context = context;
        public readonly IMediator _mediator = mediator;
        private readonly IGoogleService _googleService = googleService;
        private readonly TozawaNGO.Auth.Services.ICurrentUserService _currentUserService = currentUserService;

        public async Task<TableDataDto<Models.Dtos.Backend.MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ApplicationUser> members = _context.TzUsers
                .Include(x => x.Partner);

            var result = await members
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var converted = MemberConverter.Convert(result.Where(x => request.IncludeDeleted || !x.Deleted));

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
                    var translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == member.DescriptionTextId);
                    if (translation != null)
                    {
                        if (translation.LanguageText.ContainsKey(_currentUserService.LanguageId))
                        {
                            member.Description = translation.LanguageText[_currentUserService.LanguageId];
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

                    var image = member.Attachments.FirstOrDefault(x => !string.IsNullOrEmpty(x.MiniatureId));

                    if (image != null && !string.IsNullOrEmpty(image.MiniatureId))
                    {
                        var stream = await _googleService.StreamFromGoogleFileByFolder(member.Email, image.MiniatureId);
                        var bytes = FileUtil.ReadAllBytesFromStream(stream);
                        if (bytes != null)
                        {
                            member.Thumbnail = Convert.ToBase64String(bytes);
                        }
                    }
                    foreach (var attachment in ownerAttachment.Attachments)
                    {
                        if (!string.IsNullOrEmpty(attachment.MiniatureId))
                        {
                            var stream = await _googleService.StreamFromGoogleFileByFolder(member.Email, attachment.MiniatureId);
                            var bytes = FileUtil.ReadAllBytesFromStream(stream);
                            if (bytes != null)
                            {
                                attachment.MiniatureBlobUrl = Convert.ToBase64String(bytes);
                            }
                        }
                    }
                }
            }
        }
    }
}