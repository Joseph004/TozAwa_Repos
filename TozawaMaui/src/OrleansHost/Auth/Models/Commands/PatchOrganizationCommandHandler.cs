

using MediatR;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains.Helpers;
using Grains.Auth.Models.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Grains.Auth.Controllers;
using Grains;
using Grains.Auth.Models.Authentication;

namespace OrleansHost.Auth.Models.Commands
{
    public class PatchOrganizationCommand : IRequest<OrganizationDto>
    {
        public Guid Id { get; set; }
        public JsonPatchDocument PatchModel { get; set; }
    }
    public class PatchOrganizationCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, IMediator mediator
    , IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<PatchOrganizationCommandHandler> logger) : IRequestHandler<PatchOrganizationCommand, OrganizationDto>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<PatchOrganizationCommandHandler> _logger = logger;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<OrganizationDto> Handle(PatchOrganizationCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var organization = await _context.Organizations.Include(u => u.Addresses)
                           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);


            if (organization == null || (!_currentUserService.User.Roles.Any(x => x.Role == Grains.Auth.Models.Dtos.Role.VicePresident)))
            {
                _logger.LogWarning("Organization not found {id}", request.Id);
                throw new Exception(nameof(request));
            }

            if (request.PatchModel.GetPatchValue<bool>("DeleteForever"))
            {
                var translationId = organization.DescriptionTextId;
                var commentTranslationId = organization.CommentTextId;
                var organizationId = organization.Id;
                var translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == translationId, cancellationToken: cancellationToken);
                var commentTranslation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == commentTranslationId, cancellationToken: cancellationToken);
                var ownerAttachements = await _context.OwnerFileAttachments.Include(x => x.FileAttachment).Where(y => y.OwnerId == organizationId).ToListAsync(cancellationToken: cancellationToken);
                var attachementIds = ownerAttachements.Select(x => x.FileAttachmentId).Distinct();

                _context.Organizations.Remove(organization);

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

                await _factory.GetGrain<IOrganizationGrain>(organizationId).ClearAsync();
                await _hub.Clients.All.SendAsync("OrganizationUpdated", organizationId, true, cancellationToken: cancellationToken);
                return OrganizationConverter.Convert(organization, organization.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Address = a.Address,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    Active = a.Active
                }).ToList());
            }

            if (request.PatchModel.GetPatchValue<string>("Description") != null)
            {
                var translation = new Grains.Auth.Models.Authentication.Translation();
                if (organization.Description != request.PatchModel.GetPatchValue<string>("Description"))
                {
                    organization.Description = request.PatchModel.GetPatchValue<string>("Description");
                    if (organization.DescriptionTextId == Guid.Empty)
                    {
                        var id = Guid.NewGuid();
                        organization.DescriptionTextId = id;
                        translation = new Grains.Auth.Models.Authentication.Translation
                        {
                            Id = Guid.NewGuid(),
                            TextId = id,
                            LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Description") } }
                        };
                        _context.Translations.Add(translation);
                    }
                    else
                    {
                        translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == organization.DescriptionTextId, cancellationToken: cancellationToken);
                        if (translation != null)
                        {
                            translation.LanguageText[_currentUserService.LanguageId] = request.PatchModel.GetPatchValue<string>("Description");
                            _context.Entry(translation).State = EntityState.Modified;
                        }
                        else
                        {
                            var id = organization.DescriptionTextId;
                            translation = new Grains.Auth.Models.Authentication.Translation
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

            request.PatchModel.ApplyTo(organization);

            _context.SaveChanges();
            var attachmentsCount = await context.FileAttachments.Include(t => t.Owners).CountAsync(x => x.Owners.Any(y => y.OwnerId == organization.Id), cancellationToken);
            var item = new OrganizationItem(
                 organization.Id,
            organization.Name,
            organization.City,
            organization.Email,
            organization.PhoneNumber,
            organization.CreatedBy,
            organization.CreateDate,
            organization.ModifiedBy,
            organization.ModifiedDate,
            organization.Features.Select(x => x.Feature).ToList(),
            attachmentsCount,
            organization.Id,
            organization.Comment,
            organization.CommentTextId,
            organization.Description,
            organization.DescriptionTextId,
            organization.Deleted
            );
            var OrganizationDto = OrganizationConverter.Convert(organization, organization.Addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Name = a.Name,
                Address = a.Address,
                City = a.City,
                State = a.State,
                Country = a.Country,
                ZipCode = a.ZipCode,
                Active = a.Active
            }).ToList());
            OrganizationDto.AttachmentsCount = attachmentsCount;
            await _factory.GetGrain<IOrganizationGrain>(organization.Id).SetAsync(item);
            await _hub.Clients.All.SendAsync("OrganizationUpdated", organization.Id, false, cancellationToken: cancellationToken);

            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.UpdateOrganization,
                UserName = organization.Name,
                Email = organization.Email
            });
            _context.SaveChanges();

            return OrganizationDto;
        }
    }
}