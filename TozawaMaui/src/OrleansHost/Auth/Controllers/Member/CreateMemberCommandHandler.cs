
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
using Grains.Services;
using OrleansHost.Helpers;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Controllers
{
    public class CreateMemberCommandHandler(TozawangoDbContext context, UserManager<ApplicationUser> userManager, IEmailMessageService emailMessageService, ICurrentUserService currentUserService, ILookupNormalizer normalizer,
    IGrainFactory factory, IHubContext<ClientHub> hub, ILogger<CreateMemberCommandHandler> logger) : IRequestHandler<CreateMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailMessageService _emailMessageService = emailMessageService;
        private readonly IGrainFactory _factory = factory;
        private readonly IHubContext<ClientHub> _hub = hub;
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<CreateMemberCommandHandler> _logger = logger;
        private readonly ILookupNormalizer _normalizer = normalizer;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Models.Dtos.Backend.MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin())
            {
                throw new UnauthorizedAccessException("user not allowed to update");
            }

            var existingMember = await _context.TzUsers.Include(u => u.Addresses)
                           .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

            if (existingMember != null && !existingMember.Deleted)
            {
                _logger.LogWarning("Member email already existing request {email}", request.Email);
                throw new ArgumentException("Member email already existing");
            }
            var partner = _context.Partners.First(x => x.Email == "tozawango@gmail.com");
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AdminMember = false,
                UserCountry = request.Country,
                PartnerId = partner.Id,
                Partner = partner,
                Addresses = request.Addresses.Select(x => new UserAddress
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
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.Email);
            _context.TzUsers.Add(newuser);

            var translation = new Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Description))
            {
                AddTranslation(request.Description, _currentUserService.LanguageId, newuser, translation);
            }

            foreach (var desc in request.DescriptionTranslations)
            {
                AddTranslation(desc.Text, desc.LanguageId, newuser, translation);
            }
            if (translation.Id != Guid.Empty)
            {
                _context.Translations.Add(translation);
                var itemTranslation = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                 translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            var commentTranslation = new Models.Authentication.Translation();
            if (!string.IsNullOrEmpty(request.Comment))
            {
                AddComentTranslation(request.Comment, _currentUserService.LanguageId, newuser, commentTranslation);
            }

            foreach (var comment in request.CommentTranslations)
            {
                AddComentTranslation(comment.Text, comment.LanguageId, newuser, commentTranslation);
            }
            if (commentTranslation.Id != Guid.Empty)
            {
                _context.Translations.Add(commentTranslation);
                var itemTranslation = new TranslationItem(commentTranslation.Id, commentTranslation.TextId, commentTranslation.LanguageText, SystemTextId.TranslationOwnerId,
                 commentTranslation.CreatedBy, commentTranslation.CreateDate, commentTranslation.ModifiedBy, commentTranslation.ModifiedDate ?? new DateTime());
                await _factory.GetGrain<ITranslationGrain>(itemTranslation.TextId).SetAsync(itemTranslation);
            }

            _context.SaveChanges();

            var item = new MemberItem(
                newuser.UserId,
      newuser.PartnerId,
      newuser.Description,
     newuser.DescriptionTextId,
      newuser.FirstName,
      newuser.LastName,
      newuser.LastLoginCountry,
      newuser.LastLoginCity,
      newuser.LastLoginState,
      newuser.LastLoginIPAdress,
      newuser.Adress,
      newuser.UserPasswordHash,
      request.Roles,
      newuser.LastAttemptLogin,
      newuser.RefreshToken,
      newuser.RefreshTokenExpiryTime,
      newuser.UserCountry,
      newuser.Deleted,
      newuser.AdminMember,
      newuser.LastLogin,
      newuser.CreatedBy,
      newuser.CreateDate,
      newuser.ModifiedBy,
      newuser.ModifiedDate,
      newuser.StationIds,
      newuser.Email,
      newuser.PasswordHash,
      0,
      newuser.Tenants,
      newuser.LandLords,
      request.Features,
      request.Functions,
      newuser.Comment,
      newuser.CommentTextId,
      SystemTextId.MemberOwnerId
            );
            await _factory.GetGrain<IMemberGrain>(newuser.UserId).SetAsync(item);
            await _hub.Clients.All.SendAsync("MemberAdded", newuser.UserId, cancellationToken: cancellationToken);

            var newpassword = await CreateNewPassword(newuser);
            if (!string.IsNullOrEmpty(newpassword))
            {
                await _emailMessageService.SendNewPassword(request.Email, newpassword);
            }
            _context.UserLogs.Add(new UserLog
            {
                Event = LogEventType.AddUser,
                UserName = newuser.UserName,
                Email = newuser.Email
            });
            _context.SaveChanges();
            var addressDto = newuser.Addresses.Select(x => new AddressDto
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
            return MemberConverter.Convert(newuser, addressDto);
        }

        private static void AddTranslation(string text, Guid languageId, ApplicationUser newuser, Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                newuser.Description = text;
                if (newuser.DescriptionTextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newuser.DescriptionTextId = id;
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
                        var id = newuser.DescriptionTextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }

        private static void AddComentTranslation(string text, Guid languageId, ApplicationUser newuser, Models.Authentication.Translation translation)
        {
            if (text != null)
            {
                newuser.Comment = text;
                if (newuser.CommentTextId == Guid.Empty)
                {
                    var id = Guid.NewGuid();
                    newuser.CommentTextId = id;
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
                        var id = newuser.CommentTextId;

                        translation.Id = Guid.NewGuid();
                        translation.TextId = id;
                        translation.LanguageText = new Dictionary<Guid, string> { { languageId, text } };
                    }
                }
            }
        }

        private async Task<string> CreateNewPassword(ApplicationUser user)
        {
            var newPassword = GeneratePassword.RandomPassword();

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
            var res = await _userManager.UpdateAsync(user);
            if (res.Succeeded)
            {
                return newPassword;
            }

            return "";
        }
    }
}