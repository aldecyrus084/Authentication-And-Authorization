using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Domain.Entities
{
    public class Users
    {
        public Guid userId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string HashPass { get; private set; }

        protected Users() { }

        public Users(string name, string email, string hashPassword)
        {
            Name = name;
            Email = email;
            HashPass = hashPassword;
        }
    }
}
