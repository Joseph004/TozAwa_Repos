using Tozawa.Attachment.Svc.Clients;
using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Handlers.Commands;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Tests.Helpers;
using Tozawa.Core.Common.Middleware;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Commands.FileAttachment;

public  class UpdateAttachmentCommandHandlerTests : DatabaseContextDependencyTestsBase
{
#nullable disable
    private UpdateAttachmentCommandHandler target;

    private Mock<IFileAttachmentConverter> _fileAttachmentConverterMock;
    private Mock<IDocumentBlobClient> _documentBlobClientMock;
#nullable enable

    private readonly string testFile = "TestData/testfile.txt";

    [SetUp]
    public void Setup()
    {
        _documentBlobClientMock = new Mock<IDocumentBlobClient>();
        _fileAttachmentConverterMock = new Mock<IFileAttachmentConverter>();

        target = new UpdateAttachmentCommandHandler(Context,
            _fileAttachmentConverterMock.Object,
            _documentBlobClientMock.Object,
            CurrentUserService.Object);
    }

    [Test]
    public void Handle_Update_UnknownOrganizationId_Throws()
    {
        var id = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            OrganizationId = Guid.NewGuid()
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var command = new UpdateAttachmentCommand { Id = id };
        Assert.ThrowsAsync<NotFoundStatusCodeException>(
            () => target.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Update_Updates()
    {
        var id = Guid.NewGuid();
        var blobId = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            BlobId = blobId,
            OrganizationId = OrganizationId,
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var command = new UpdateAttachmentCommand
        {
            Id = id,
            File = FormFileHelper.GetIFormFileFromFile(testFile),
            OwnerIds = new () { Guid.NewGuid() }
        };
        await target.Handle(command, CancellationToken.None);

        _documentBlobClientMock
            .Verify(dbcm => dbcm.Update(blobId, It.IsAny<byte[]>()), Times.Once);
    }
}
