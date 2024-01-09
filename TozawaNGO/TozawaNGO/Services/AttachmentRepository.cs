using MediatR;
using TozawaNGO.Context;
using TozawaNGO.HttpClients;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Services;
public interface IAttachmentRepository
{
    Task<AddResponse<List<FileAttachmentDto>>> AddAttachment();
}
public class AttachmentRepository : IAttachmentRepository
{
    private readonly IMediator _mediator;
    public AttachmentRepository(IMediator mediator) => _mediator = mediator;

    public async Task<AddResponse<List<FileAttachmentDto>>> AddAttachment()
    {
        return await Task.FromResult(new AddResponse<List<FileAttachmentDto>>(false, "", System.Net.HttpStatusCode.InternalServerError, null));
    }
}

