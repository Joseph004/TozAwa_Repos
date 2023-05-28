using Tozawa.Attachment.Svc.Clients;
using Tozawa.Attachment.Svc.Handlers.Queries.Blob;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Tests.Helpers;
using Tozawa.Core.Common.Middleware;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Queries.Blob;

[TestFixture]
public class GetBlobQueryHandlerTests : DatabaseContextDependencyTestsBase
{
#nullable disable
    private GetBlobQueryHandler target;

    private Mock<IDocumentBlobClient> _documentBlobClientMock;
#nullable enable

    [SetUp]
    public void Setup()
    {
        _documentBlobClientMock = new Mock<IDocumentBlobClient>();
        _documentBlobClientMock
            .Setup(dbc => dbc.Get(It.IsAny<Guid>()))
            .ReturnsAsync(new byte[1]);

        target = new GetBlobQueryHandler(_documentBlobClientMock.Object,
            CurrentUserService.Object,
            Context);
    }

    [Test]
    public void Handle_Get_UnknownOrganizationId_Throws()
    {
        var blobId = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            BlobId = blobId,
            OrganizationId = Guid.NewGuid()
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetBlobQuery(blobId);
        Assert.ThrowsAsync<NotFoundStatusCodeException>(
            () => target.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Get_CallsGet()
    {
        var blobId = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            BlobId = blobId,
            OrganizationId = OrganizationId
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetBlobQuery(blobId);
        await target.Handle(query, CancellationToken.None);

        _documentBlobClientMock
            .Verify(dbcm => dbcm.Get(blobId), Times.Once);
    }
}
