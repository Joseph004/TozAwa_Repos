

using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace TozawaNGO.Auth.Controllers
{
    public class PatchMemberCommand : IRequest<Models.Dtos.Backend.MemberDto>
    {
        public Guid Id { get; set; }
        public JsonPatchDocument PatchModel { get; set; }
    }
}