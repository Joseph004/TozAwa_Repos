using MediatR;
using Tozawa.Bff.Portal.Helper;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, TableDataDto<MemberDto>>
    {
        private readonly IMemberService _MemberService;
        private readonly IMemberConverter _converter;
        private readonly IAttachmentHttpClient _attachmentHttpClient;
        private readonly IGoogleService _googleService;

        public GetMembersQueryHandler(IMemberService MemberService, IMemberConverter converter, IAttachmentHttpClient attachmentHttpClient, IGoogleService googleService)
        {
            _MemberService = MemberService;
            _converter = converter;
            _attachmentHttpClient = attachmentHttpClient;
            _googleService = googleService;
        }
        public async Task<TableDataDto<MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            var Members = await _MemberService.GetItems();
            var converted = _converter.Convert(Members.Where(x => request.IncludeDeleted || !x.Deleted));

            if (!string.IsNullOrEmpty(request.PageOfEmail))
            {
                var itemWithEmail = converted.FirstOrDefault(x => x.Email == request.PageOfEmail);
                if (itemWithEmail != null)
                {
                    var itemIndex = converted.IndexOf(itemWithEmail);
                    var itemPage = (int)Math.Floor((double)itemIndex / request.PageSize);
                    var pagedItems = converted.Skip(itemPage * request.PageSize).Take(request.PageSize);
                    await GetAttachments(pagedItems);
                    return new TableDataDto<MemberDto> { Items = pagedItems, TotalItems = converted.Count, ItemPage = itemPage };
                }
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                converted = converted.Where(x => x.Email.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
                await GetAttachments(converted);
                return new TableDataDto<MemberDto> { Items = converted, TotalItems = converted.Count };
            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                converted = converted.Where(Filtered(request.SearchString)).ToList();
            }
            var paged = converted.Skip(request.Page * request.PageSize).Take(request.PageSize);
            await GetAttachments(paged);
            return new TableDataDto<MemberDto> { Items = paged, TotalItems = converted.Count };
        }
        private static Func<MemberDto, bool> Filtered(string searchString) => x => x.Email.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                                                               (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                                (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                                (!string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

        private async Task GetAttachments(IEnumerable<MemberDto> members)
        {
            var ownerAttachments = await _attachmentHttpClient.Post<List<OwnerAttachments>>("fileattachment/owners", new { OwnerIds = members.Select(x => x.Id).ToList() });
            
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