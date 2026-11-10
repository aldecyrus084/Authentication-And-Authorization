using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Domain.Entities
{
    public class Permissions
    {
        public Guid PermissionId { get; private set; }
        public string Name { get; private set; }

        protected Permissions() { }

        public ICollection<RolePermission> RolePermissions { get; private set } = new List<RolePermission>();

        public Permissions(string name)
        {
            Name = name;
        }
    }
}
