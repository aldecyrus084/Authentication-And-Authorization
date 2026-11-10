using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Domain.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Roles Role { get; private set; }
        public Guid PermissionId { get; private set; }
        public Permissions Permission { get; private set; }
    }
}
