using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Grains.Auth.Models.Authentication
{
    public class Translation : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TextId { get; set; }
        public Dictionary<Guid, string> LanguageText { get; set; }
    }

    internal class TranslationEntityConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.Property(e => e.LanguageText).HasConversion<DictionaryGuidStringCoverter, DictionaryGuidStringComparer>();
        }
    }
}