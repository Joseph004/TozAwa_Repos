using Tozawa.Attachment.Svc.Clients;
using Tozawa.Attachment.Svc.Handlers.Commands;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Tests.Helpers;
using Tozawa.Core.Common.Middleware;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Commands.Blob;

[TestFixture]
public class DeleteBlobCommandHandlerTests : DatabaseContextDependencyTestsBase
{
#nullable disable
    private DeleteBlobCommandHandler target;

    private Mock<IDocumentBlobClient> _documentBlobClientMock;
#nullable enable

    [SetUp]
    public void Setup()
    {
        _documentBlobClientMock = new Mock<IDocumentBlobClient>();
        _documentBlobClientMock
            .Setup(dbc => dbc.Get(It.IsAny<Guid>()))
            .ReturnsAsync(new byte[1]);

        target = new DeleteBlobCommandHandler(_documentBlobClientMock.Object,
            CurrentUserService.Object,
            Context);
    }

    [Test]
    public void Handle_Delete_UnknownOrganizationId_Throws()
    {
        var blobId = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            BlobId = blobId,
            OrganizationId = Guid.NewGuid()
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var command = new DeleteBlobCommand(blobId);
        Assert.ThrowsAsync<NotFoundStatusCodeException>(
            () => target.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Delete_CallsDelete()
    {
        var blobId = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            BlobId = blobId,
            OrganizationId = OrganizationId
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var command = new DeleteBlobCommand(blobId);
        await target.Handle(command, CancellationToken.None);

        _documentBlobClientMock
            .Verify(dbcm => dbcm.Delete(blobId), Times.Once);
    }
}
