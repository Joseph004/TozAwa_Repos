

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Context;
using OrleansHost.Extension;
using OrleansHost.Auth.Models.Converters;
using OrleansHost.Auth.Services;
using Microsoft.Extensions.Logging;

namespace OrleansHost.Auth.Controllers
{
    public class PatchMemberCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, ILogger<PatchMemberCommandHandler> logger) : IRequestHandler<PatchMemberCommand, Models.Dtos.Backend.MemberDto>
    {
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
                _context.TzUsers.Remove(member);
                _context.SaveChanges();
                return MemberConverter.Convert(member, true);
            }

            if (request.PatchModel.GetPatchValue<string>("Description") != null)
            {
                if (member.Description != request.PatchModel.GetPatchValue<string>("Description"))
                {
                    member.Description = request.PatchModel.GetPatchValue<string>("Description");
                    if (member.DescriptionTextId == Guid.Empty)
                    {
                        var id = Guid.NewGuid();
                        member.DescriptionTextId = id;
                        _context.Translations.Add(new Models.Authentication.Translation
                        {
                            Id = Guid.NewGuid(),
                            TextId = id,
                            LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Description") } }
                        });
                    }
                    else
                    {
                        var translation = await _context.Translations.FirstOrDefaultAsync(x => x.TextId == member.DescriptionTextId, cancellationToken: cancellationToken);
                        if (translation != null)
                        {
                            translation.LanguageText[_currentUserService.LanguageId] = request.PatchModel.GetPatchValue<string>("Description");
                            _context.Entry(translation).State = EntityState.Modified;
                        }
                        else
                        {
                            var id = member.DescriptionTextId;
                            _context.Translations.Add(new Models.Authentication.Translation
                            {
                                Id = Guid.NewGuid(),
                                TextId = id,
                                LanguageText = new Dictionary<Guid, string> { { _currentUserService.LanguageId, request.PatchModel.GetPatchValue<string>("Description") } }
                            });
                        }
                    }
                }
            }

            request.PatchModel.ApplyTo(member);

            _context.SaveChanges();

            return MemberConverter.Convert(member);
        }
    }
}