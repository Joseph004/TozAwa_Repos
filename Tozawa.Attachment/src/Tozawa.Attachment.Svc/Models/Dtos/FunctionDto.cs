using System;
using Newtonsoft.Json;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.extension;

namespace Tozawa.Attachment.Svc.Models.Dtos
{
    public class FunctionDto
    {
        [JsonConverter(typeof(FunctionTypeConverter))]
        public FunctionType FunctionType { get; set; }

        public FunctionTypeValues FunctionTypeValues { get; set; }
        public Guid RoleId { get; set; }
    }
}
