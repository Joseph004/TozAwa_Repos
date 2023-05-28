using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.Request.Backend;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers
{
#nullable enable

    public class PatchMemberCommandHandler : PatchHandler, IRequestHandler<PatchMemberCommand, UpdateResponse<MemberDto>>
    {
        private readonly IMemberService _memberService;
        private readonly IMemberConverter _converter;
        private readonly ITozAwaAuthHttpClient _client;

        public PatchMemberCommandHandler(IMemberService memberService, IMemberConverter converter, ITozAwaAuthHttpClient client, IMediator mediator, ILogger<PatchMemberCommand> logger, ICurrentUserService currentUserService)
            : base(logger, currentUserService, mediator)
        {
            _memberService = memberService;
            _converter = converter;
            _client = client;
        }
        public async Task<UpdateResponse<MemberDto>> Handle(PatchMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var member = await _memberService.GetItem(request.Id);

                if (member == null)
                {
                    _logger.LogWarning("Member not found {id}", request.Id);
                    throw new Exception(nameof(request));
                }

                var uri = $"member/{request.Id}";
                var query = new Models.Request.Frontend.PatchMemberRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Deleted = request.Deleted,
                    DeleteForever = request.DeleteForever
                };

                var patchDocRequest = new JsonPatchDocument();

                patchDocRequest = new PatchMemberRequest(query).ToPatchDocument();

                var response = new UpdateResponse<Models.Dtos.Backend.MemberDto>(false, "", null);
                var updateResponse = new Models.Dtos.Backend.MemberDto();

                if (patchDocRequest.Operations.Count > 0)
                {
                    updateResponse = await _client.Patch<Models.Dtos.Backend.MemberDto>(uri, patchDocRequest);
                    await HandlePatch(patchDocRequest, request.Id, UpdateEntityType.Member, cancellationToken);
                }
                else
                {
                    return new UpdateResponse<MemberDto>(true, UpdateMessages.Success, _converter.Convert(member));
                }

                if (updateResponse == null)
                {
                    return new UpdateResponse<MemberDto>(false, UpdateMessages.Error, null);
                }

                return new UpdateResponse<MemberDto>(true, UpdateMessages.Success, _converter.Convert(updateResponse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Member");
                return new UpdateResponse<MemberDto>(false, UpdateMessages.Error, null);
            }
        }
    }
}