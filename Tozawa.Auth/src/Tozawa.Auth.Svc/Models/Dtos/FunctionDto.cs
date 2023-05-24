using System;
using Newtonsoft.Json;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.Models.Dtos
{
    public class FunctionDto
    {
        [JsonConverter(typeof(FunctionTypeConverter))]
        public FunctionType FunctionType { get; set; }

        public FunctionTypeValues FunctionTypeValues { get; set; }
        public Guid RoleId { get; set; }
    }
}
