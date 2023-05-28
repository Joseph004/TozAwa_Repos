using System;
using System.Collections.Generic;
using System.Linq;

namespace Tozawa.Client.Portal.Models.Dtos
{
   public class RoleDto
    {
        public Guid Id { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid TextId { get; set; }
    }
}


