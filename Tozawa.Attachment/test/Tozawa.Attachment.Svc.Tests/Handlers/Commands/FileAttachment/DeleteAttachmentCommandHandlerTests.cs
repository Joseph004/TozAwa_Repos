using Tozawa.Attachment.Svc.Handlers.Commands;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Tests.Helpers;
using Tozawa.Core.Common.Middleware;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Commands.FileAttachment;

public  class DeleteAttachmentCommandHandlerTests  : DatabaseContextDependencyTestsBase
{
#nullable disable
    private DeleteAttachmentCommandHandler target;
#nullable enable

    [SetUp]
    public void Setup()
    {
        target = new DeleteAttachmentCommandHandler(Context,
            CurrentUserService.Object);
    }

    [Test]
    public void Handle_Delete_UnknownOrganizationId_Throws()
    {
        var id = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            OrganizationId = Guid.NewGuid()
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var command = new DeleteAttachmentCommand { Id = id };
        Assert.ThrowsAsync<NotFoundStatusCodeException>(
            () => target.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Delete_Deletes()
    {
        var id = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            OrganizationId = OrganizationId
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();
        
        var command = new DeleteAttachmentCommand { Id = id };
        await target.Handle(command, CancellationToken.None);

        Assert.IsFalse(Context.FileAttachments.Any());
    }
}
