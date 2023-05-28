using Tozawa.Attachment.Svc.Converters;
using Tozawa.Attachment.Svc.Handlers.Queries.FileAttachment;
using Tozawa.Attachment.Svc.Models.Queries;
using Tozawa.Attachment.Svc.Tests.Helpers;
using Tozawa.Core.Common.Middleware;

namespace Tozawa.Attachment.Svc.Tests.Handlers.Queries.FileAttachment;

public  class GetAttachmentQueryHandlerTests : DatabaseContextDependencyTestsBase
{
#nullable disable
    private GetAttachmentQueryHandler target;

    private Mock<IFileAttachmentConverter> _attachmentConverterMock;
#nullable enable

    [SetUp]
    public void Setup()
    {
        _attachmentConverterMock = new Mock<IFileAttachmentConverter>();
        _attachmentConverterMock
            .Setup(dbc => dbc.Convert(It.IsAny<Context.Models.FileAttachment>()))
            .Returns((Context.Models.FileAttachment attachment) => 
                new Models.Dtos.FileAttachmentDto
                {
                    Id = attachment.Id,
                });

        target = new GetAttachmentQueryHandler(Context,
            _attachmentConverterMock.Object,
            CurrentUserService.Object);
    }

    [Test]
    public void Handle_Get_UnknownOrganizationId_Throws()
    {
        var id = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            OrganizationId = Guid.NewGuid()
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetAttachmentQuery(id);
        Assert.ThrowsAsync<NotFoundStatusCodeException>(
            () => target.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_Get_Returns()
    {
        var id = Guid.NewGuid();
        var fileAttachment = new Context.Models.FileAttachment
        {
            Id = id,
            OrganizationId = OrganizationId
        };

        Context.Add(fileAttachment);
        Context.BaseSaveChanges();

        var query = new GetAttachmentQuery(id);
        var dto = await target.Handle(query, CancellationToken.None);

        _attachmentConverterMock
            .Verify(acm => acm.Convert(fileAttachment), Times.Once);
        Assert.That(dto.Id, Is.EqualTo(fileAttachment.Id));
    }
}
