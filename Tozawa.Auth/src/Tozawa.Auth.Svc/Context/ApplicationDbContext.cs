using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Services;

namespace Tozawa.Auth.Svc.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext(ICurrentUserService currentUserService, DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<OrganizationFeature> OrganizationFeatures { get; set; }
        public DbSet<TozAwaFeature> TozAwaFeatures { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<UserHashPwd> userHashPwds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Authorization");

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<Organization>()
                .Property(x => x.LanguageIds)
                .HasListGuidConverter()
                .IsRequired()
                .HasDefaultValueSql("''");

            builder.Entity<Organization>()
                .Property(x => x.ExportLanguageIds)
                .HasListGuidConverter()
                .IsRequired()
                .HasDefaultValueSql("''");

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
                .HasPrincipalKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserHashPwd>()
            .HasOne(t => t.ApplicationUser)
            .WithOne(u => u.UserHashPwd)
            .HasPrincipalKey<ApplicationUser>(y => y.UserId)
            .HasForeignKey<UserHashPwd>(j => j.UserId);

            builder.Entity<UserRole>()
                .HasOne(pt => pt.Role)
                .WithMany(t => t.Users)
                .HasForeignKey(pt => pt.RoleId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
               .HasMany(p => p.Organizations)
               .WithMany(p => p.OrganizationUsers)
               .UsingEntity<UserOrganization>(
               j => j
                   .HasOne(pt => pt.Organization)
                   .WithMany(t => t.UserOrganizations)
                   .HasForeignKey(pt => pt.OrganizationId)
                   .HasPrincipalKey(x => x.Id)
                   .OnDelete(DeleteBehavior.Restrict),
               j => j
                   .HasOne(pt => pt.User)
                   .WithMany(p => p.UserOrganizations)
                   .HasForeignKey(pt => pt.UserId)
                   .HasPrincipalKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict),
               j =>
               {
                   j.HasKey(t => new { t.OrganizationId, t.UserId });
               });


            builder.Entity<PartnerOrganization>()
                .HasKey(e => new { e.FromId, e.ToId });

            builder.Entity<PartnerOrganization>()
                .HasOne(e => e.OrganizationFrom)
                .WithMany(e => e.PartnerOrganizationsTo)
                .HasForeignKey(e => e.FromId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartnerOrganization>()
                .HasOne(e => e.OrganizationTo)
                .WithMany(e => e.PartnerOrganizationsFrom)
                .HasForeignKey(e => e.ToId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<ApplicationUser>()
                .HasOne(p => p.Organization)
                .WithMany(b => b.Users)
                .HasForeignKey(p => p.OrganizationId);

            builder.Entity<UserRole>()
                .Property(x => x.Active)
                .HasDefaultValue(true);


            builder.Entity<TozAwaFeature>().Property(p => p.Id).ValueGeneratedNever();
            builder.Entity<TozAwaFeature>().HasData(
                new TozAwaFeature
                {
                    Id = 1,
                    TextId = Guid.Parse("acd1ef02-0da3-474b-95d3-8861fcc8e368"),
                    DescriptionTextId = Guid.Parse("97617538-8931-43ee-bd4c-769726bdb6a4")
                },
                new TozAwaFeature
                {
                    Id = 2,
                    TextId = Guid.Parse("4368eb54-8521-4892-b8fb-b4993ca115e2"),
                    DescriptionTextId = Guid.Parse("9319cd54-bc74-48cb-bcfc-7c7266dce102")
                },
                new TozAwaFeature
                {
                    Id = 3,
                    TextId = Guid.Parse("f2f40352-5e9f-4fc9-baab-201b0b9285fd"),
                    DescriptionTextId = Guid.Parse("56afa5ba-1dbb-4220-bff7-667bf0669986")
                },
                new TozAwaFeature
                {
                    Id = 4,
                    TextId = Guid.Parse("d88e4649-a599-499b-a711-99e747ef621f"),
                    DescriptionTextId = Guid.Parse("2307af30-b6a1-41cc-979c-762f599aea1c")
                },
                new TozAwaFeature
                {
                    Id = 5,
                    TextId = Guid.Parse("051daa8c-4e74-4a23-a1de-c91bbd11075a"),
                    DescriptionTextId = Guid.Parse("27596311-463e-4994-81c5-a5ae83cb3768")
                },
                new TozAwaFeature
                {
                    Id = 6,
                    TextId = Guid.Parse("fe7530e2-d275-44b1-9565-d31964c7688b"),
                    DescriptionTextId = Guid.Parse("d7c29ad0-26d6-4c28-b269-de7373814f40")
                },
                new TozAwaFeature
                {
                    Id = 7,
                    TextId = Guid.Parse("e185f011-cd98-47e2-a918-df370965481c"),
                    DescriptionTextId = Guid.Parse("ba218979-4758-4a15-be35-7874f5e84d52")
                },
                new TozAwaFeature
                {
                    Id = 8,
                    TextId = Guid.Parse("892837fb-4949-48f1-9407-954bba0894f7"),
                    DescriptionTextId = Guid.Parse("443d5204-02fe-4569-969f-fb452816c332")
                },
                new TozAwaFeature
                {
                    Id = 9,
                    TextId = Guid.Parse("a95fea80-24f5-45af-9012-1555c0d2d241"),
                    DescriptionTextId = Guid.Parse("7859194f-b86c-4bcd-891a-c99f5d33db63")
                },
                new TozAwaFeature
                {
                    Id = 10,
                    TextId = Guid.Parse("b89fbc25-a81e-4f8d-ba37-0b96817d878d"),
                    DescriptionTextId = Guid.Parse("cc91eeaa-26ff-4249-86e2-dfb7f9580b3c")
                },
                new TozAwaFeature
                {
                    Id = 11,
                    TextId = Guid.Parse("67f38c1d-f365-4d63-9939-aa91d4a15cb4"),
                    DescriptionTextId = Guid.Parse("ff737bcd-c475-4620-9a47-131021f23d2b")
                },
                new TozAwaFeature
                {
                    Id = 12,
                    TextId = Guid.Parse("4653d990-6f5e-4e00-a52a-7f57aa94d2fc"),
                    DescriptionTextId = Guid.Parse("50c553e7-665e-492e-abe9-1a9e50133996")
                },
                new TozAwaFeature
                {
                    Id = 13,
                    TextId = Guid.Parse("cc391b4d-0c0e-44fe-a343-23129c7166ee"),
                    DescriptionTextId = Guid.Parse("00054fe9-290e-467c-a19e-ed259cb9c1e0")
                },
                new TozAwaFeature
                {
                    Id = 14,
                    TextId = Guid.Parse("2685e7a0-8ff0-49d7-997b-12e50c333ffe"),
                    DescriptionTextId = Guid.Parse("4058300a-5b7b-42a7-b0bb-ccbf68d32d52")
                },
                new TozAwaFeature
                {
                    Id = 15,
                    TextId = Guid.Parse("27dd2f68-9656-477a-8cb0-068e230291c0"),
                    DescriptionTextId = Guid.Parse("8209d9e8-2deb-4120-9f91-31619ae422b1")
                },
                new TozAwaFeature
                {
                    Id = 16,
                    TextId = Guid.Parse("e56902fa-77b5-41a7-b74c-17eb111342e3"),
                    DescriptionTextId = Guid.Parse("fac2671f-688d-48bb-af06-0f3f0c8bd407")
                },
                new TozAwaFeature
                {
                    Id = 17,
                    TextId = Guid.Parse("80a0d18f-2d7b-4579-b741-9485c5265e13"),
                    DescriptionTextId = Guid.Parse("3fcda399-dd92-43e2-87a9-61d7a671c778")
                },
                new TozAwaFeature
                {
                    Id = 18,
                    TextId = Guid.Parse("d10c571e-fb30-4a1c-8115-9d5343045dd4"),
                    DescriptionTextId = Guid.Parse("7258b643-40e4-416b-b3cc-5db3f71a4abe")
                },
                new TozAwaFeature
                {
                    Id = 19,
                    TextId = Guid.Parse("d1338999-d588-4448-aa73-9b62414cb7b6"),
                    DescriptionTextId = Guid.Parse("c3dde4a8-1287-4321-8ebf-b918da35d013")
                },
                new TozAwaFeature
                {
                    Id = 21,
                    TextId = Guid.Parse("36cd3946-ab37-4476-b824-9442fbcc9b55"),
                    DescriptionTextId = Guid.Parse("c0ed5988-b532-4884-abeb-1aaf9b1118cf")
                },
                new TozAwaFeature
                {
                    Id = 22,
                    TextId = Guid.Parse("93698a14-eaa7-4649-a3aa-4eb3080a6961"),
                    DescriptionTextId = Guid.Parse("be98612e-569f-4558-9d16-db2d3e2675ca")
                },
                new TozAwaFeature
                {
                    Id = 23,
                    TextId = Guid.Parse("c75ddc1a-eb3c-42bb-9a7d-597eb146a3be"),
                    DescriptionTextId = Guid.Parse("2a0dd5af-c377-4d15-a8ae-642c55252d61")
                },
                new TozAwaFeature
                {
                    Id = 24,
                    TextId = Guid.Parse("32e1ad3b-c319-4e6e-864d-01c02e8138b5"),
                    DescriptionTextId = Guid.Parse("71dc6d6c-193d-4334-b56d-f91f77faaf9a")
                },
                new TozAwaFeature
                {
                    Id = 25,
                    TextId = Guid.Parse("6a506b08-86be-4305-bc3f-a16fdb12556d"),
                    DescriptionTextId = Guid.Parse("96a2ce3e-74a5-45dc-8377-5faeb78d296d")
                },
                new TozAwaFeature
                {
                    Id = 26,
                    TextId = Guid.Parse("a7a366a6-8a3d-4621-b053-57cbedff427d"),
                    DescriptionTextId = Guid.Parse("7a7692df-6da4-47d8-a147-e63998328461")
                }
            );
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
                    OrganizationId = _currentUserService.User != null ? _currentUserService.User.OrganizationId : Guid.NewGuid(),
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
            public Guid OrganizationId { get; set; }
            public string TableName { get; set; } = "";
            public DateTime DateTime { get; set; }
            public string KeyValues { get; set; } = "";
            public string OldValues { get; set; } = "";
            public string NewValues { get; set; } = "";
        }
    }
}