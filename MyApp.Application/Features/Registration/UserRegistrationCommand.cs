using MediatR;
using MyApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Features.Registration
{
    public class UserRegistrationCommand : IRequest<GenericResponseDTO<UserRegistrationResponseDTO>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
