using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using TozawaNGO.Auth.Models;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Services;

namespace TozawaNGO.Context
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

        public DbSet<ApplicationUser> TzUsers { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<UserHashPwd> UserHashPwds { get; set; }

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

            builder.Entity<UserHashPwd>()
            .HasOne(t => t.ApplicationUser)
            .WithOne(u => u.UserHashPwd)
            .HasPrincipalKey<ApplicationUser>(y => y.UserId)
            .HasForeignKey<UserHashPwd>(j => j.UserId);

            builder.Entity<ApplicationUser>()
                .HasOne(p => p.Partner)
                .WithMany(b => b.Users)
                .HasForeignKey(p => p.PartnerId);
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
            return base.SaveChanges(); ;
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

        public class AuditEntry
        {
            private readonly ICurrentUserService _currentUserService;

            public AuditEntry(EntityEntry entry, ICurrentUserService currentUserService)
            {
                _currentUserService = currentUserService;
                Entry = entry;
            }

            public EntityEntry Entry { get; }
            public string TableName { get; set; }
            public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
            public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
            public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
            public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

            public bool HasTemporaryProperties => TemporaryProperties.Any();

            public Audit ToAudit()
            {
                var audit = new Audit
                {
                    PartnerId = _currentUserService.User != null ? _currentUserService.User.PartnerId : Guid.NewGuid(),
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
            public string TableName { get; set; } = "";
            public DateTime DateTime { get; set; }
            public string KeyValues { get; set; } = "";
            public string OldValues { get; set; } = "";
            public string NewValues { get; set; } = "";
        }
    }
}