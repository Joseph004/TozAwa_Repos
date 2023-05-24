using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Context
{
    public class AttachmentContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        protected AttachmentContext(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public AttachmentContext(DbContextOptions<AttachmentContext> options) : base(options)
        {
        }

        public AttachmentContext(ICurrentUserService currentUserService, DbContextOptions<AttachmentContext> options) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public virtual DbSet<ConvertedOwner> ConvertedOwners { get; set; }
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }
        public virtual DbSet<OwnerFileAttachment> OwnerFileAttachments { get; set; }

        [Obsolete("This is only used for convertion of old data. Will be removed in near feature.")]
        public virtual DbSet<ImageInformation> ImagesInformation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Attachment");

            builder.Entity<OwnerFileAttachment>().HasKey(x => new
            {
                x.OwnerId,
                x.FileAttachmentId
            });

            builder.Entity<OwnerFileAttachment>()
                .HasOne(x => x.FileAttachment)
                .WithMany(x => x.Owners)
                .HasForeignKey(x => x.FileAttachmentId);

            builder.Entity<FileAttachment>()
                .HasMany(x => x.Owners)
                .WithOne(x => x.FileAttachment)
                .HasForeignKey(x => x.FileAttachmentId);

            builder.Entity<FileAttachment>()
                .HasIndex(x => x.BlobId);

            builder.Entity<ImageInformation>().Property(t => t.Width).HasColumnType("decimal(18,4)");
            builder.Entity<ImageInformation>().Property(t => t.Height).HasColumnType("decimal(18,4)");
        }

        public int BaseSaveChanges()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges()
        {
            HandleAccessAndMetadata();

            var result = base.SaveChanges();

            return result;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleAccessAndMetadata();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private void HandleAccessAndMetadata()
        {
            var deletedModified = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified);

            foreach (var entityEntry in deletedModified)
            {
                if (entityEntry.Entity is BaseEntity entry && entry.OrganizationId != _currentUserService.User.OrganizationId)
                {
                    throw new AccessViolationException(nameof(_currentUserService.User.OrganizationId));
                }
            }

            var addedOrModified = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Modified);

            foreach (var dbEntityEntry in addedOrModified)
            {
                if (dbEntityEntry.Entity is BaseEntity organizationEntity && _currentUserService.User != null)
                {
                    SetCreatedAndModifiedAndOrganizationId(dbEntityEntry, organizationEntity);
                }
            }
        }

        private void SetCreatedAndModifiedAndOrganizationId(EntityEntry dbEntityEntry, BaseEntity entity)
        {
            switch (dbEntityEntry.State)
            {
                case EntityState.Added:
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.CreatedBy = _currentUserService.User.UserName;
                    entity.OrganizationId = _currentUserService.User.OrganizationId;
                    break;
                case EntityState.Modified:
                    entity.ModifiedDate = DateTime.UtcNow;
                    entity.ModifiedBy = _currentUserService.User.UserName;
                    break;
            }
        }
    }
}