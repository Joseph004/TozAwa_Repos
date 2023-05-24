using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Tozawa.Language.Svc.Context
{
    public class SystemType
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<Translation> Translations { get; set; } = new HashSet<Translation>();
    }

    internal class SystemTypeConfiguration : IEntityTypeConfiguration<SystemType>
    {
        public void Configure(EntityTypeBuilder<SystemType> entity)
        {
            entity.ToTable("SystemTypes", "Tozawa.Language");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("('2018-09-18T17:52:25.138Z')");
        }
    }
}
