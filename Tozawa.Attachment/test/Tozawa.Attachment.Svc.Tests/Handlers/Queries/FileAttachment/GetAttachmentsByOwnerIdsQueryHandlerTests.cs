using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Tests.Helpers;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandlerTests : DatabaseContextDependencyTestsBase
{
#nullable disable
    private GetAttachmentsByOwnerIdsQueryHandler target;

    private Mock<IFileAttachmentConverter> _attachmentConverterMock;
#nullable enable

    [SetUp]
    public void Setup()
    {
        _attachmentConverterMock = new Mock<IFileAttachmentConverter>();
        _attachmentConverterMock
            .Setup(dbc => dbc.Convert(It.IsAny<Context.Models.FileAttachment>()))
            .Returns(new Models.Dtos.FileAttachmentDto());

        target = new GetAttachmentsByOwnerIdsQueryHandler(Context,
            _attachmentConverterMock.Object,
            CurrentUserService.Object);
    }

    [Test]
    public async Task Handle_Get_UnknownOrganizationId_Empty()
    {
        var fileAttachmentId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var attachementTypeId = Guid.NewGuid();
        var owners = new List<Context.Models.OwnerFileAttachment>
        {
            new Context.Models.OwnerFileAttachment
            {
                OwnerId = ownerId
            }
        };
        var fileAttachmentType = new Context.Models.FileAttachmentType
        {
            Id = attachementTypeId,
            Name = "fileAttachmentType",
            CreatedBy = "createdBy"
        };

        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = fileAttachmentId,
            OrganizationId = Guid.NewGuid(),
            Owners = owners,
            FileAttachmentType = fileAttachmentType
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetAttachmentsByOwnerIdsQuery
        {
            OwnerIds = new () { ownerId },
        };
        Assert.IsEmpty(await target.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Get_Returns()
    {
        var fileAttachmentId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var attachementTypeId = Guid.NewGuid();

        var owners = new List<Context.Models.OwnerFileAttachment>
        {
            new Context.Models.OwnerFileAttachment
            {
                OwnerId = ownerId
            }
        };
        var fileAttachmentType = new Context.Models.FileAttachmentType
        {
            Id = attachementTypeId,
            Name = "fileAttachmentType",
            CreatedBy = "createdBy"
        };

        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = fileAttachmentId,
            OrganizationId = OrganizationId,
            Owners = owners,
            FileAttachmentType = fileAttachmentType
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetAttachmentsByOwnerIdsQuery
        {
            OwnerIds = new() { ownerId },
        };
        var fileAttachments = await target.Handle(query, CancellationToken.None);

        Assert.That(fileAttachments.Count, Is.EqualTo(1));
        _attachmentConverterMock
            .Verify(acm => acm.Convert(It.IsAny<Context.Models.FileAttachment>()), Times.Once);
    }
}
