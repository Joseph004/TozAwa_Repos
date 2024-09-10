

using MediatR;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains.Helpers;

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
            var member = await _context.TzUsers
                           .FirstOrDefaultAsync(x => x.UserId == request.Id, cancellationToken: cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Member not found {id}", request.Id);
                throw new Exception(nameof(request));
            }

            if (request.PatchModel.GetPatchValue<bool>("DeleteForever"))
            {
                var translationId = member.DescriptionTextId;
                var memberId = member.UserId;
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
                return MemberConverter.Convert(member, true);
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

            var item = new MemberItem(
                member.UserId,
      member.PartnerId,
      member.Description,
     member.DescriptionTextId,
      member.FirstName,
      member.LastName,
      member.LastLoginCountry,
      member.LastLoginCity,
      member.LastLoginState,
      member.LastLoginIPAdress,
      member.Adress,
      member.UserPasswordHash,
      member.Roles,
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
      SystemTextId.MemberOwnerId
            );
            var memberDto = MemberConverter.Convert(member);
            await _factory.GetGrain<IMemberGrain>(member.UserId).SetAsync(item);
            await _hub.Clients.All.SendAsync("MemberUpdated", member.UserId, false, cancellationToken: cancellationToken);

            return memberDto;
        }
    }
}