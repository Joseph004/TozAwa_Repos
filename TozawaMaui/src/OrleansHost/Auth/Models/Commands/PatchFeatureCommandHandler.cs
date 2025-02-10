

using MediatR;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
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
    public class PatchFeatureCommand : IRequest<FeatureDto>
    {
        public int Id { get; set; }
        public JsonPatchDocument PatchModel { get; set; }
    }
    public class PatchFeatureCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, IMediator mediator
    , IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<PatchFeatureCommandHandler> logger) : IRequestHandler<PatchFeatureCommand, FeatureDto>
    {
        private readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<PatchFeatureCommandHandler> _logger = logger;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<FeatureDto> Handle(PatchFeatureCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var feature = await _context.TozawaFeatures
                           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);


            if (feature == null || (!_currentUserService.User.Roles.Any(x => x.Role == Grains.Auth.Models.Dtos.Role.VicePresident)))
            {
                _logger.LogWarning("Feature not found {id}", request.Id);
                throw new Exception(nameof(request));
            }

            if (request.PatchModel.GetPatchValue<string>("Description") != null)
            {
                var translation = new Grains.Auth.Models.Authentication.Translation();
                var translationItem = await _factory.GetGrain<ITranslationGrain>(feature.DescriptionTextId).GetAsync();
                if (translationItem != null && translationItem.LanguageText[_currentUserService.LanguageId] != request.PatchModel.GetPatchValue<string>("Description"))
                {
                    if (feature.DescriptionTextId == Guid.Empty)
                    {
                        var id = Guid.NewGuid();
                        feature.DescriptionTextId = id;
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
                        translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == feature.DescriptionTextId, cancellationToken: cancellationToken);
                        if (translation != null)
                        {
                            translation.LanguageText[_currentUserService.LanguageId] = request.PatchModel.GetPatchValue<string>("Description");
                            _context.Entry(translation).State = EntityState.Modified;
                        }
                        else
                        {
                            var id = feature.DescriptionTextId;
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

            if (request.PatchModel.GetPatchValue<string>("Text") != null)
            {
                var translation = new Grains.Auth.Models.Authentication.Translation();
                var translationItem = await _factory.GetGrain<ITranslationGrain>(feature.TextId).GetAsync();
                if (translationItem != null && translationItem.LanguageText[_currentUserService.LanguageId] != request.PatchModel.GetPatchValue<string>("Text"))
                {
                    if (feature.TextId == Guid.Empty)
                    {
                        var id = Guid.NewGuid();
                        feature.TextId = id;
                        translation = new Grains.Auth.Models.Authentication.Translation
                        {
                            Id = Guid.NewGuid(),
                            TextId = id,
                            LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Text") } }
                        };
                        _context.Translations.Add(translation);
                    }
                    else
                    {
                        translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == feature.TextId, cancellationToken: cancellationToken);
                        if (translation != null)
                        {
                            translation.LanguageText[_currentUserService.LanguageId] = request.PatchModel.GetPatchValue<string>("Text");
                            _context.Entry(translation).State = EntityState.Modified;
                        }
                        else
                        {
                            var id = feature.TextId;
                            translation = new Grains.Auth.Models.Authentication.Translation
                            {
                                Id = Guid.NewGuid(),
                                TextId = id,
                                LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Text") } }
                            };
                            _context.Translations.Add(translation);
                        }
                        var itemTranslation = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                        translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate ?? new DateTime());
                        await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
                    }
                }
            }

            request.PatchModel.ApplyTo(feature);

            _context.SaveChanges();
            var item = new FeatureItem(
                   feature.Id,
            feature.TextId,
            feature.DescriptionTextId,
            feature.Deleted,
            SystemTextId.FeatureOwnerId
            );
            var featureDto = new FeatureDto
            {
                Id = feature.Id,
                TextId = feature.TextId,
                DescriptionTextId = feature.DescriptionTextId,
                Deleted = feature.Deleted
            };
            await _factory.GetGrain<IFeatureGrain>(feature.TextId).SetAsync(item);
            await _hub.Clients.All.SendAsync("FeatureUpdated", feature.Id, false, cancellationToken: cancellationToken);

            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.UpdateFeature,
                DescriptionTextId = featureDto.DescriptionTextId,
                TextId = featureDto.TextId
            });
            _context.SaveChanges();

            return featureDto;
        }
    }
}