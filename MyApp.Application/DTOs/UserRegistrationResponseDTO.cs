using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.DTOs
{
    public class UserRegistrationResponseDTO
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
