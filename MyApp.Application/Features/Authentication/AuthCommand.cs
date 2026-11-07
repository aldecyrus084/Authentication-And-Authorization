using MediatR;
using MyApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Features.Authentication
{
    public class AuthCommand : IRequest<GenericResponseDTO<AuthResponseDTO>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
