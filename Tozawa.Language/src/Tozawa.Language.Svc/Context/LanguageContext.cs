using Microsoft.EntityFrameworkCore;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Context
{
    public class LanguageContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public LanguageContext()
        {
        }

        public LanguageContext(DbContextOptions<LanguageContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public virtual DbSet<Languagemd> Languages { get; set; }
        public virtual DbSet<SystemType> SystemTypes { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<XliffDistributionFiles> XliffDistributionFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public override int SaveChanges()
        {
            try
            {
                var distinctSystemTypeIds = ChangeTracker
                    .Entries()
                    .Where(x => x.Entity is Translation)
                    .Select(x => x.Entity)
                    .Select(x => (x as Translation).SystemTypeId)
                    .Distinct()
                    .ToList();

                var lastChanged = DateTime.UtcNow;
                foreach (var distinctSystemTypeId in distinctSystemTypeIds)
                {
                    var systemType = SystemTypes.Find(distinctSystemTypeId);
                    if (systemType != null) systemType.LastUpdated = lastChanged;
                }
                var added = ChangeTracker.Entries().Where(p => p.State == EntityState.Added);
                foreach (var dbEntityEntry in added)
                {
                    if (dbEntityEntry.Entity is BaseEntity baseEntity)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        baseEntity.CreatedBy = _currentUserService.User.UserName;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var distinctSystemTypeIds = ChangeTracker
                    .Entries()
                    .Where(x => x.Entity is Translation)
                    .Select(x => x.Entity)
                    .Select(x => (x as Translation).SystemTypeId)
                    .Distinct()
                    .ToList();

                var lastChanged = DateTime.UtcNow;
                foreach (var distinctSystemTypeId in distinctSystemTypeIds)
                {
                    var systemType = SystemTypes.Find(distinctSystemTypeId);
                    if (systemType != null) systemType.LastUpdated = lastChanged;
                }
                var added = ChangeTracker.Entries().Where(p => p.State == EntityState.Added);
                foreach (var dbEntityEntry in added)
                {
                    if (dbEntityEntry.Entity is BaseEntity baseEntity)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Language");
            LoadEntityConfigurations(modelBuilder);
        }

        private void LoadEntityConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<XliffDistributionFiles>().HasIndex(e => e.SourceLanguageId).HasDatabaseName("IX_SourceLanguageId");
            modelBuilder.Entity<XliffDistributionFiles>().HasIndex(e => e.TargetLanguageId).HasDatabaseName("IX_TargetLanguageId");
            modelBuilder.Entity<XliffDistributionFiles>().Property(e => e.CreatedAt).HasColumnType("datetime");
            modelBuilder.Entity<XliffDistributionFiles>().Property(e => e.RequestedDeliveryDate).HasColumnType("datetime");
            modelBuilder.Entity<XliffDistributionFiles>().HasOne(d => d.SourceLanguage)
                .WithMany(p => p.XliffDistributionFilesSourceLanguage)
                .HasForeignKey(d => d.SourceLanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Language.XliffDistributionFiles_Language.Languages_SourceLanguageId");

            modelBuilder.Entity<XliffDistributionFiles>().HasOne(d => d.TargetLanguage)
                .WithMany(p => p.XliffDistributionFilesTargetLanguage)
                .HasForeignKey(d => d.TargetLanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Language.XliffDistributionFiles_Language.Languages_TargetLanguageId");


            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.StartsWith("Language"));

            var assemblyTypes = assemblies.SelectMany(x => x.GetTypes());

            assemblyTypes = assemblyTypes
                .Where(p =>
                    (p.Namespace?.StartsWith("Language") ?? false) &&
                    p.IsClass &&
                    !p.IsAbstract &&
                    p.GetInterfaces().Any(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .ToList();

            var modelConfigurations = assemblyTypes.Select(x => Activator.CreateInstance(x) as dynamic).ToList();

            modelConfigurations
                .ToList()
                .ForEach(x => modelBuilder.ApplyConfiguration(x));
        }
    }
}
