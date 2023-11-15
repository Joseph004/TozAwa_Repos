using System;

namespace TozawaNGO.Models.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; } = "";
    }
}