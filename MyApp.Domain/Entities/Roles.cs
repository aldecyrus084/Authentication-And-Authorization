using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Domain.Entities
{
    public class Roles
    {
        public Guid roleId { get; private set; }
        public string Name { get; private set; }

        protected Roles() { }

        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        public Roles(string name)
        {
            Name = name;
        }
    }
}
