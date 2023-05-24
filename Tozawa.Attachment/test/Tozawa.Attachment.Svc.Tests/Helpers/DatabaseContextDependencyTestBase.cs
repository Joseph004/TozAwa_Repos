using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Core.Common.CurrentUser;

namespace Tozawa.Attachment.Svc.Tests.Helpers;

public abstract class DatabaseContextDependencyTestsBase
{
#nullable disable
    private SqliteConnection _connection;
    protected AttachmentContext Context;
    protected Mock<ICurrentUserService> CurrentUserService;
    protected DbContextOptionsBuilder<AttachmentContext> DbContextOptionsBuilder;
    protected Guid OrganizationId;
    private DbContextOptions<AttachmentContext> _options;
#nullable enable

    [SetUp]
    public void DbContextDependencySetUp()
    {
        OrganizationId = Guid.NewGuid();
        _options = new DbContextOptionsBuilder<AttachmentContext>()
            .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
            .Options;

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();


        DbContextOptionsBuilder = new DbContextOptionsBuilder<AttachmentContext>()
            .UseSqlite(_connection);

        CurrentUserService = new Mock<ICurrentUserService>();
        CurrentUserService.Setup(x => x.User).Returns(new UserDto
        {
            OrganizationId = OrganizationId
        });

        Context = CreateNewContext();
        Context.Database.EnsureCreated();
    }

    protected AttachmentContext CreateNewContext()
    {
        return new AttachmentContext(CurrentUserService.Object, _options);
    }

    protected Mock<ICurrentUserService> GetCurrentUserServiceMock()
    {
        return CurrentUserService;
    }

    protected void ClearContext()
    {
        Context = CreateNewContext();
    }

    [TearDown]
    public void DatabaseTearDown()
    {
        Context.Database.EnsureDeleted();
        _connection.Close();
    }
}
