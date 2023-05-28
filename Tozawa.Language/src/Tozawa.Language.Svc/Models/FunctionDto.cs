using System;
using Newtonsoft.Json;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;

namespace Tozawa.Language.Svc.Models
{
    public class FunctionDto
    {
        [JsonConverter(typeof(FunctionTypeConverter))]
        public FunctionType FunctionType { get; set; }

        public FunctionTypeValues FunctionTypeValues { get; set; }
        public Guid RoleId { get; set; }
    }
}
