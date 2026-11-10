using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; private set; }
        public Users User { get; private set; }
        public Guid RoleId { get; private set; }
        public Roles Role { get; private set; }
    }
}
