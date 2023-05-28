using System;
using System.Collections.Generic;
using System.Linq;

namespace TozAwaHome.Models.Dtos
{
   public class RoleDto
    {
        public Guid Id { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid TextId { get; set; }
    }
}


