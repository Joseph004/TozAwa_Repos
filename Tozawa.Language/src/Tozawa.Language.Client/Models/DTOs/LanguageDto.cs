using System;
using System.ComponentModel.DataAnnotations;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class LanguageDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Shortname should be between 3-10 characters")]
        public string ShortName { get; set; } = "";

        [Required]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Name should be between 3-25 characters.")]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Long name should be between 3-25 characters.")]
        public string LongName { get; set; } = "";

        public bool Deleted { get; set; }
        public bool IsDefault { get; set; }
    }
}
