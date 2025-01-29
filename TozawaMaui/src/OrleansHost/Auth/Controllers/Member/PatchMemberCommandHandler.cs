

using MediatR;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains.Helpers;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers
{
    public class PatchMemberCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, IMediator mediator
    , IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<PatchMemberCommandHandler> logger) : IRequestHandler<PatchMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<PatchMemberCommandHandler> _logger = logger;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Models.Dtos.Backend.MemberDto> Handle(PatchMemberCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var member = await _context.TzUsers
                    .Include(t => t.UserOrganizations)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.OrganizationUsers)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.Features)
                    .Include(w => w.Addresses)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role.Functions)
                    .FirstOrDefaultAsync(x => x.UserId == request.Id, cancellationToken: cancellationToken);


            if (member == null || (!_currentUserService.User.Roles.Any(x => x.Role == Grains.Auth.Models.Dtos.Role.VicePresident) && member.AdminMember) || (member.UserId == _currentUserService.User.Id && !_currentUserService.User.Roles.Any(r => r.Role == Grains.Auth.Models.Dtos.Role.President)))
            {
                _logger.LogWarning("Member not found {id}", request.Id);
                throw new Exception(nameof(request));
            }
            var memberItem = await _factory.GetGrain<IMemberGrain>(member.UserId).GetAsync();

            if (request.PatchModel.GetPatchValue<bool>("DeleteForever"))
            {
                var translationId = member.DescriptionTextId;
                var memberId = member.UserId;
                var commentTranslationId = member.CommentTextId;
                var commentTranslation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == commentTranslationId, cancellationToken: cancellationToken);
                var translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == translationId, cancellationToken: cancellationToken);
                var ownerAttachements = await _context.OwnerFileAttachments.Include(x => x.FileAttachment).Where(y => y.OwnerId == memberId).ToListAsync(cancellationToken: cancellationToken);
                var attachementIds = ownerAttachements.Select(x => x.FileAttachmentId).Distinct();

                _context.TzUsers.Remove(member);

                foreach (var ownerAttach in ownerAttachements)
                {
                    if (ownerAttach.FileAttachment != null)
                    {
                        _context.FileAttachments.RemoveRange(ownerAttach.FileAttachment);
                    }
                }
                _context.OwnerFileAttachments.RemoveRange(ownerAttachements);
                if (translation != null)
                {
                    _context.Translations.Remove(translation);
                }
                if (commentTranslation != null)
                {
                    _context.Translations.Remove(commentTranslation);
                }
                _context.SaveChanges();

                foreach (var attachId in attachementIds)
                {
                    await _factory.GetGrain<IAttachmentGrain>(attachId).ClearAsync();
                }
                if (translationId != Guid.Empty)
                {
                    await _factory.GetGrain<ITranslationGrain>(translationId).ClearAsync();
                }

                await _factory.GetGrain<IMemberGrain>(memberId).ClearAsync();
                await _hub.Clients.All.SendAsync("MemberUpdated", memberId, true, cancellationToken: cancellationToken);

                _context.UserLogs.Add(new UserLog
                {
                    Event = LogEventType.DeleteUser,
                    UserName = member.UserName,
                    Email = member.Email
                });
                _context.SaveChanges();

                var address = member.Addresses.Select(x => new AddressDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    ZipCode = x.ZipCode,
                    Active = x.Active
                }).ToList();
                return MemberConverter.Convert(memberItem, [], [], [], [], true);
            }

            if (request.PatchModel.GetPatchValue<string>("Description") != null)
            {
                var translation = new Models.Authentication.Translation();
                if (member.Description != request.PatchModel.GetPatchValue<string>("Description"))
                {
                    member.Description = request.PatchModel.GetPatchValue<string>("Description");
                    if (member.DescriptionTextId == Guid.Empty)
                    {
                        var id = Guid.NewGuid();
                        member.DescriptionTextId = id;
                        translation = new Models.Authentication.Translation
                        {
                            Id = Guid.NewGuid(),
                            TextId = id,
                            LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Description") } }
                        };
                        _context.Translations.Add(translation);
                    }
                    else
                    {
                        translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == member.DescriptionTextId, cancellationToken: cancellationToken);
                        if (translation != null)
                        {
                            translation.LanguageText[_currentUserService.LanguageId] = request.PatchModel.GetPatchValue<string>("Description");
                            _context.Entry(translation).State = EntityState.Modified;
                        }
                        else
                        {
                            var id = member.DescriptionTextId;
                            translation = new Models.Authentication.Translation
                            {
                                Id = Guid.NewGuid(),
                                TextId = id,
                                LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Description") } }
                            };
                            _context.Translations.Add(translation);
                        }
                        var itemTranslation = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                        translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate ?? new DateTime());
                        await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
                    }
                }
            }

            request.PatchModel.ApplyTo(member);

            _context.SaveChanges();
            var attachmentsCount = await context.FileAttachments.Include(t => t.Owners).CountAsync(x => x.Owners.Any(y => y.OwnerId == member.UserId), cancellationToken);
            var item = new MemberItem(
                member.UserId,
      member.Description,
     member.DescriptionTextId,
      member.FirstName,
      member.LastName,
      member.LastLoginCountry,
      member.LastLoginCity,
      member.LastLoginState,
      member.LastLoginIPAdress,
      member.Roles.Select(r => r.Role.RoleEnum).ToList(),
      member.LastAttemptLogin,
      member.RefreshToken,
      member.RefreshTokenExpiryTime,
      member.UserCountry,
      member.Deleted,
      member.AdminMember,
      member.LastLogin,
      member.CreatedBy,
      member.CreateDate,
      member.ModifiedBy,
      member.ModifiedDate,
      member.StationIds,
      member.Email,
      member.PasswordHash,
      attachmentsCount,
      member.Tenants.Select(x => x.UserTenant_TenantUser.UserId).ToList(),
      member.LandLords.Select(x => x.UserLandLord_LandLordUser.UserId).ToList(),
      member.Organizations?.SelectMany(o => o.Features) != null
            ? member.Organizations?.SelectMany(o => o.Features).Select(x => x.Feature).ToList()
            : [],
      member.Roles
                .SelectMany(x => x.Role.Functions)
                .Select(function => function.FunctionType)
                .Distinct()
                .ToList(),
      member.Comment,
      member.CommentTextId,
      member.UserOrganizations?.Select(x => x.OrganizationId).ToList(),
      member.CityCode,
      member.CountryCode,
      member.Gender,
      SystemTextId.MemberOwnerId
            );
            await _factory.GetGrain<IMemberGrain>(member.UserId).SetAsync(item);
            var addressDtos = member.Addresses.Select(x => new AddressDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Country = x.Country,
                ZipCode = x.ZipCode,
                Active = x.Active
            }).ToList();
            var organizations = new List<CurrentUserOrganizationDto>();
            foreach (var orgItem in member.UserOrganizations)
            {
                var org = (await _mediator.Send(new GetOrganizationQuery { Id = orgItem.OrganizationId })) ?? new();
                organizations.Add(new CurrentUserOrganizationDto
                {
                    Id = org.Id,
                    Addresses = org.Addresses,
                    Features = org.Features,
                    Name = org.Name,
                    Active = true
                });
            }
            var roleDtos = await _mediator.Send(new GetRolesQuery(member.UserId));
            var functions = roleDtos.SelectMany(x => x.Functions).Select(y => new CurrentUserFunctionDto
            {
                FunctionType = y.FunctionType
            }).Distinct().ToList();
            var memberDto = MemberConverter.Convert(item, organizations, addressDtos, roleDtos.ToList(), functions);

            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.UpdateUser,
                UserName = memberDto.UserName,
                Email = memberDto.Email
            });
            _context.SaveChanges();

            memberDto.AttachmentsCount = attachmentsCount;
            await _hub.Clients.All.SendAsync("MemberUpdated", member.UserId, false, cancellationToken: cancellationToken);

            return memberDto;
        }
    }
}