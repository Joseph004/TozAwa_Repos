using System;

namespace TozAwaHome.Models.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }
}