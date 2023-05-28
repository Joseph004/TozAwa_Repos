using System;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.extension;
using Tozawa.Bff.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Models
{
    public class FunctionDto
    {
        [JsonConverter(typeof(FunctionTypeConverter))]
        public FunctionType FunctionType { get; set; }

        public FunctionTypeValues FunctionTypeValues { get; set; }
        public Guid RoleId { get; set; }
    }
}
