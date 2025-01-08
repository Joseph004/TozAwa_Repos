
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
    public class CreateOrganizationCommand : IRequest<OrganizationDto>
    {
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "Sweden";
        public Guid DescriptionId { get; set; }
        public string Description { get; set; } = "";
        public string Comment { get; set; } = "";
        public List<int> Features { get; set; } = [];
        public List<RoleEnum> Roles { get; set; } = [];
        public List<AddressCommand> Addresses { get; set; } = [];
        public List<FunctionType> Functions { get; set; } = [];
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
        public List<TranslationRequest> CommentTranslations { get; set; } = [];
    }

    public class AddressCommand
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
    }
    public class CreateOrganizationCommandRequestFluentValidator : AbstractValidator<CreateOrganizationCommand>
    {
        public CreateOrganizationCommandRequestFluentValidator()
        {
            RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

            RuleFor(x => x.DescriptionTranslations)
            .NotNull()
            .Must(x => x.All(y => y.LanguageId != Guid.Empty));

            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage("A valid email is required");
        }
    }
    public class TranslationRequest
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }
    public class CreateOrganizationCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, ILookupNormalizer normalizer,
    IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<CreateOrganizationCommandHandler> logger) : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly IGrainFactory _factory = factory;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<CreateOrganizationCommandHandler> _logger = logger;
        private readonly ILookupNormalizer _normalizer = normalizer;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var existingOrganization = await _context.Organizations.Include(u => u.Addresses)
                           .FirstOrDefaultAsync(x => x.Email == request.Email || (!string.IsNullOrEmpty(x.Name) && x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)), cancellationToken: cancellationToken);

            if (existingOrganization != null && !existingOrganization.Deleted)
            {
                _logger.LogWarning("Organization email or name already existing request {email}", request.Email);
                throw new ArgumentException("Organization email already existing");
            }
            var newOrganization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                City = request.City,
                Country = request.Country,
                Addresses = request.Addresses.Select(x => new OrganizationAddress
                {
                    Id = Guid.NewGuid(),
                    Name = x.Name,
                    Address = x.Address,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    ZipCode = x.ZipCode,
                    Active = x.Active
                }).ToList()
            };
            _context.Organizations.Add(newOrganization);

            var translation = new Grains.Auth.Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Description))
            {
                AddTranslation(request.Description, _currentUserService.LanguageId, newOrganization, translation);
            }

            foreach (var desc in request.DescriptionTranslations)
            {
                AddTranslation(desc.Text, desc.LanguageId, newOrganization, translation);
            }
            if (translation.Id != Guid.Empty)
            {
                _context.Translations.Add(translation);
                var itemTranslation = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                 translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            var commentTranslation = new Grains.Auth.Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Comment))
            {
                AddComentTranslation(request.Comment, _currentUserService.LanguageId, newOrganization, commentTranslation);
            }

            foreach (var comment in request.CommentTranslations)
            {
                AddComentTranslation(comment.Text, comment.LanguageId, newOrganization, commentTranslation);
            }
            if (commentTranslation.Id != Guid.Empty)
            {
                _context.Translations.Add(commentTranslation);
                var itemTranslation = new TranslationItem(commentTranslation.Id, commentTranslation.TextId, commentTranslation.LanguageText, SystemTextId.TranslationOwnerId,
                 commentTranslation.CreatedBy, commentTranslation.CreateDate, commentTranslation.ModifiedBy, commentTranslation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            _context.SaveChanges();

            var item = new OrganizationItem(
                newOrganization.Id,
            newOrganization.Name,
            newOrganization.City,
            newOrganization.Email,
            newOrganization.PhoneNumber,
            newOrganization.CreatedBy,
            newOrganization.CreateDate,
            newOrganization.ModifiedBy,
            newOrganization.ModifiedDate,
            newOrganization.Features.Select(x => x.Feature).ToList(),
            0,
            newOrganization.Id,
            newOrganization.Comment,
            newOrganization.CommentTextId,
            newOrganization.Description,
            newOrganization.DescriptionTextId,
            newOrganization.Deleted
            );
            await _factory.GetGrain<IOrganizationGrain>(newOrganization.Id).SetAsync(item);
            await _hub.Clients.All.SendAsync("OrganizationAdded", newOrganization.Id, cancellationToken: cancellationToken);

            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.AddOrganization,
                UserName = newOrganization.Name,
                Email = newOrganization.Email
            });
            _context.SaveChanges();

            return OrganizationConverter.Convert(newOrganization, newOrganization.Addresses.Select(a => new AddressDto
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

        private static void AddTranslation(string text, Guid languageId, Organization newOrganization, Grains.Auth.Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                newOrganization.Description = text;
                if (newOrganization.DescriptionTextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newOrganization.DescriptionTextId = id;
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
                        var id = newOrganization.DescriptionTextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }

        private static void AddComentTranslation(string text, Guid languageId, Organization newOrganization, Grains.Auth.Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                newOrganization.Comment = text;
                if (newOrganization.CommentTextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newOrganization.CommentTextId = id;
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
                        var id = newOrganization.CommentTextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }
    }
}