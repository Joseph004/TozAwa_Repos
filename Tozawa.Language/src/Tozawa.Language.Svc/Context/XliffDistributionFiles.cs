using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Tozawa.Language.Svc.Context
{
    public class XliffDistributionFiles : BaseEntity
    {
        public XliffDistributionFiles()
        {

        }
        public int Id { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public XliffFileState FileState { get; set; }
        public Guid SourceLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }
        public int NumberOfTranslations { get; set; }
        public int? NumberOfWordsSentInSourceLanguage { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
        public virtual Languagemd SourceLanguage { get; set; }
        public virtual Languagemd TargetLanguage { get; set; }
    }

    internal class XliffDistributionFilesConfiguration : IEntityTypeConfiguration<XliffDistributionFiles>
    {
        public void Configure(EntityTypeBuilder<XliffDistributionFiles> entity)
        {
            entity.ToTable("XliffDistributionFiles", "Language");

            entity.HasIndex(e => e.SourceLanguageId).HasDatabaseName("IX_SourceLanguageId");
            entity.HasIndex(e => e.TargetLanguageId).HasDatabaseName("IX_TargetLanguageId");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RequestedDeliveryDate).HasColumnType("datetime");
            entity.HasOne(d => d.SourceLanguage)
                .WithMany(p => p.XliffDistributionFilesSourceLanguage)
                .HasForeignKey(d => d.SourceLanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Language.XliffDistributionFiles_Language.Languages_SourceLanguageId");

            entity.HasOne(d => d.TargetLanguage)
                .WithMany(p => p.XliffDistributionFilesTargetLanguage)
                .HasForeignKey(d => d.TargetLanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Language.XliffDistributionFiles_Language.Languages_TargetLanguageId");
        }
    }
}
