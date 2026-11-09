using MyApp.Application.DTOs;
using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Interfaces.Services
{
    public interface ITokenGenerator
    {
        AuthResponseDTO GenerateToken(Users user);
    }
}
