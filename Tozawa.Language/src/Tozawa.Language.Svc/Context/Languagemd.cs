using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Tozawa.Language.Svc.Context
{
    public class Languagemd : BaseEntity
    {
        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public bool Deleted { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<Translation> Translations { get; set; } = new HashSet<Translation>();
        public virtual ICollection<XliffDistributionFiles> XliffDistributionFilesSourceLanguage { get; set; } = new HashSet<XliffDistributionFiles>();
        public virtual ICollection<XliffDistributionFiles> XliffDistributionFilesTargetLanguage { get; set; } = new HashSet<XliffDistributionFiles>();
    }

    internal class LanguageConfiguration : IEntityTypeConfiguration<Languagemd>
    {
        public void Configure(EntityTypeBuilder<Languagemd> entity)
        {
            entity.ToTable("Languages", "Language");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        }
    }
}
