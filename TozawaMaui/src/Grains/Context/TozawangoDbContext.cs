using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Grains.Attachment.Models;
using Grains.Auth.Models;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Services;

namespace Grains.Context
{
    public class TozawangoDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        public TozawangoDbContext(DbContextOptions<TozawangoDbContext> options) : base(options)
        {
        }
        public TozawangoDbContext(ICurrentUserService currentUserService, DbContextOptions<TozawangoDbContext> options) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Role> TzRoles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<UserRole> TzUserRoles { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<OrganizationAddress> OrganizationAddresses { get; set; }
        public DbSet<StationAddress> StationAddresses { get; set; }
        public DbSet<OrganizationFeature> OrganizationFeatures { get; set; }
        public DbSet<TozawaFeature> TozawaFeatures { get; set; }
        public DbSet<ApplicationUser> TzUsers { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public virtual DbSet<ConvertedOwner> ConvertedOwners { get; set; }
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }
        public virtual DbSet<OwnerFileAttachment> OwnerFileAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Authorization");

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<ApplicationUser>()
                .HasIndex(u => new { u.Id, u.UserId, u.Email })
                .IsUnique();

            builder.Entity<ApplicationUser>()
                .HasOne(p => p.Partner)
                .WithMany(b => b.Users)
                .HasForeignKey(p => p.PartnerId);

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

            builder.Entity<Function>().HasKey(x => new
            {
                x.Functiontype,
                x.RoleId
            });
            builder.Entity<UserRole>()
                .HasKey(t => new { t.UserId, t.RoleId });

            builder.Entity<UserRole>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.Roles)
                .HasForeignKey(pt => pt.UserId)
                .HasPrincipalKey(k => k.UserId);

            builder.Entity<UserRole>()
                .HasOne(pt => pt.Role)
                .WithMany(t => t.Users)
                .HasForeignKey(pt => pt.RoleId)
                .HasPrincipalKey(k => k.Id);

            builder.Entity<ApplicationUser>()
               .HasMany(p => p.Organizations)
               .WithMany(p => p.OrganizationUsers)
               .UsingEntity<UserOrganization>(
               j => j
                   .HasOne(pt => pt.Organization)
                   .WithMany(t => t.UserOrganizations)
                   .HasForeignKey(pt => pt.OrganizationId)
                   .HasPrincipalKey(k => k.Id),
               j => j
                   .HasOne(pt => pt.User)
                   .WithMany(p => p.UserOrganizations)
                   .HasForeignKey(pt => pt.UserId)
                   .HasPrincipalKey(k => k.UserId),
               j =>
               {
                   j.HasKey(t => new { t.OrganizationId, t.UserId });
               });

            builder.Entity<ApplicationUser>()
                .HasOne(p => p.Organization)
                .WithMany(b => b.Users)
                .HasForeignKey(p => p.OrganizationId);

            builder.Entity<UserAddress>()
            .HasOne(x => x.User)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(k => k.UserId);

            builder.Entity<TozawaFeature>().Property(p => p.Id).ValueGeneratedNever();
            builder.Entity<TozawaFeature>().HasData(
                new TozawaFeature
                {
                    Id = 1,
                    TextId = Guid.Parse("acd1ef02-0da3-474b-95d3-8861fcc8e368"),
                    DescriptionTextId = Guid.Parse("97617538-8931-43ee-bd4c-769726bdb6a4")
                }
            );

            builder.ApplyConfiguration(new TranslationEntityConfiguration());
            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }

        public int BaseSaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        public int SaveChangesForTestsOnly()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges()
        {
            var addedOrModified = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Modified);
            foreach (var dbEntityEntry in addedOrModified)
            {
                if (dbEntityEntry.Entity is CreatedModified createdModified)
                {
                    SetCreatedAndModifiedAndOrgranizationId(dbEntityEntry, createdModified);
                }
                else if (dbEntityEntry.Entity is CreatedNotModified createdNotModified)
                {
                    SetCreated(dbEntityEntry, createdNotModified);
                }
            }

            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        private void SetCreated(EntityEntry dbEntityEntry, CreatedNotModified entity)
        {
            if (dbEntityEntry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.CreatedBy = _currentUserService?.User != null ? _currentUserService.User.UserName : "Internal";
            }
        }

        private void SetCreatedAndModifiedAndOrgranizationId(EntityEntry dbEntityEntry, CreatedModified entity)
        {
            switch (dbEntityEntry.State)
            {
                case EntityState.Added:
                    entity.CreateDate = DateTime.UtcNow;
                    entity.CreatedBy = _currentUserService?.User != null ? _currentUserService.User.UserName : "Internal";
                    break;
                case EntityState.Modified:
                    entity.ModifiedDate = DateTime.UtcNow;
                    entity.ModifiedBy = _currentUserService?.User != null ? _currentUserService.User.UserName : "Internal";
                    break;
            }
        }

        private int OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return -1;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                if (auditEntry.NewValues != null || auditEntry.OldValues != null)
                    Audits.Add(auditEntry.ToAudit());
            }

            return BaseSaveChanges();
        }
        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry, _currentUserService) { TableName = entry.Metadata.GetTableName() };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                if (auditEntry.NewValues != null || auditEntry.OldValues != null)
                    Audits.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        public class AuditEntry(EntityEntry entry, ICurrentUserService currentUserService)
        {
            private readonly ICurrentUserService _currentUserService = currentUserService;

            public EntityEntry Entry { get; } = entry;
            public string TableName { get; set; }
            public Dictionary<string, object> KeyValues { get; } = [];
            public Dictionary<string, object> OldValues { get; } = [];
            public Dictionary<string, object> NewValues { get; } = [];
            public List<PropertyEntry> TemporaryProperties { get; } = [];

            public bool HasTemporaryProperties => TemporaryProperties.Count != 0;

            public Audit ToAudit()
            {
                var audit = new Audit
                {
                    Id = Guid.NewGuid(),
                    PartnerId = _currentUserService.User != null ? _currentUserService.User.PartnerId : Guid.NewGuid(),
                    InloggedEmail = _currentUserService.User != null ? _currentUserService.User.Email : "None",
                    TableName = TableName,
                    DateTime = DateTime.UtcNow,
                    KeyValues = JsonConvert.SerializeObject(KeyValues),
                    OldValues = OldValues.Count == 0 ? "" : JsonConvert.SerializeObject(OldValues),
                    NewValues = NewValues.Count == 0 ? "" : JsonConvert.SerializeObject(NewValues)
                };
                return audit;
            }
        }
        public class Audit
        {
            public Guid Id { get; set; }
            public Guid PartnerId { get; set; }
            public string InloggedEmail { get; set; }
            public string TableName { get; set; } = "";
            public DateTime DateTime { get; set; }
            public string KeyValues { get; set; } = "";
            public string OldValues { get; set; } = "";
            public string NewValues { get; set; } = "";
        }
    }
}