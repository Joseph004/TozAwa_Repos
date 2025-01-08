
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains.Helpers;
using Grains.Auth.Services;
using Grains.Auth.Models.Dtos;
using Grains.Models;
using FluentValidation;
using Grains;

namespace OrleansHost.Auth.Models.Commands
{
    public class CreateFeatureCommand : IRequest<FeatureDto>
    {
        public int Id { get; set; }
        public Guid TextId { get; set; }
        public Guid DescriptionTextId { get; set; }
        public string Text { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
        public List<TranslationRequest> TextTranslations { get; set; } = [];
    }

    public class CreateFeatureCommandRequestFluentValidator : AbstractValidator<CreateFeatureCommand>
    {
        public CreateFeatureCommandRequestFluentValidator()
        {
            RuleFor(x => x.Id)
            .NotNull()
            .NotEmpty();

            RuleFor(x => x.Text)
            .NotNull()
            .NotEmpty();

            RuleFor(x => x.Description)
             .NotNull()
             .NotEmpty();

            RuleFor(x => x.TextId)
          .NotNull()
          .NotEmpty();

            RuleFor(x => x.DescriptionTextId)
           .NotNull()
           .NotEmpty();
        }
    }
    public class CreateFeatureCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, ILookupNormalizer normalizer,
    IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<CreateFeatureCommandHandler> logger) : IRequestHandler<CreateFeatureCommand, FeatureDto>
    {
        private readonly IGrainFactory _factory = factory;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<CreateFeatureCommandHandler> _logger = logger;
        private readonly ILookupNormalizer _normalizer = normalizer;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<FeatureDto> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var existingFeature = await _context.TozawaFeatures
                           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (existingFeature != null && !existingFeature.Deleted)
            {
                _logger.LogWarning("Feature id already existing request {id}", request.Id);
                throw new ArgumentException("Feature email already existing");
            }
            var newFeature = new TozawaFeature
            {
                Id = request.Id,
                TextId = request.TextId,
                DescriptionTextId = request.DescriptionTextId
            };
            _context.TozawaFeatures.Add(newFeature);

            var translation = new Grains.Auth.Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Description))
            {
                AddTranslation(request.Description, _currentUserService.LanguageId, newFeature, translation);
            }

            foreach (var desc in request.DescriptionTranslations)
            {
                AddTranslation(desc.Text, desc.LanguageId, newFeature, translation);
            }
            if (translation.Id != Guid.Empty)
            {
                _context.Translations.Add(translation);
                var itemTranslation = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                 translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            var commentTranslation = new Grains.Auth.Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Text))
            {
                AddTextTranslation(request.Text, _currentUserService.LanguageId, newFeature, commentTranslation);
            }

            foreach (var text in request.TextTranslations)
            {
                AddTextTranslation(text.Text, text.LanguageId, newFeature, commentTranslation);
            }
            if (commentTranslation.Id != Guid.Empty)
            {
                _context.Translations.Add(commentTranslation);
                var itemTranslation = new TranslationItem(commentTranslation.Id, commentTranslation.TextId, commentTranslation.LanguageText, SystemTextId.TranslationOwnerId,
                 commentTranslation.CreatedBy, commentTranslation.CreateDate, commentTranslation.ModifiedBy, commentTranslation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            _context.SaveChanges();

            var item = new FeatureItem(
               newFeature.Id,
            newFeature.TextId,
            newFeature.DescriptionTextId,
            newFeature.Deleted,
            SystemTextId.FeatureOwnerId
            );
            await _factory.GetGrain<IFeatureGrain>(newFeature.TextId).SetAsync(item);
            await _hub.Clients.All.SendAsync("FeatureAdded", newFeature.Id, cancellationToken: cancellationToken);

            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.AddFeature,
                DescriptionTextId = newFeature.DescriptionTextId,
                TextId = newFeature.TextId
            });
            _context.SaveChanges();

            return new FeatureDto
            {
                Id = newFeature.Id,
                TextId = newFeature.TextId,
                DescriptionTextId = newFeature.DescriptionTextId,
                Deleted = newFeature.Deleted
            };
        }

        private static void AddTranslation(string text, Guid languageId, TozawaFeature newFeature, Grains.Auth.Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                if (newFeature.DescriptionTextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newFeature.DescriptionTextId = id;
                    translation.Id = Guid.NewGuid();
                    translation.TextId = id;
                    translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                }
                else
                {
                    if (translation != null)
                    {
                        if (translation.LanguageText == null)
                        {
                            translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                        }
                        else
                        {
                            translation.LanguageText[languageId] = text;
                        }
                    }
                    else
                    {
                        var id = newFeature.DescriptionTextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }

        private static void AddTextTranslation(string text, Guid languageId, TozawaFeature newFeature, Grains.Auth.Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                if (newFeature.TextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newFeature.TextId = id;
                    translation.Id = Guid.NewGuid();
                    translation.TextId = id;
                    translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                }
                else
                {
                    if (translation != null)
                    {
                        if (translation.LanguageText == null)
                        {
                            translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                        }
                        else
                        {
                            translation.LanguageText[languageId] = text;
                        }
                    }
                    else
                    {
                        var id = newFeature.TextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }
    }
}