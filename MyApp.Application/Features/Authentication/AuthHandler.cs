using MediatR;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces.Repository;
using MyApp.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Features.Authentication
{
    public class AuthHandler : IRequestHandler<AuthCommand, GenericResponseDTO<AuthResponseDTO>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenGenerator _tokenGen;
        public AuthHandler(IUserRepository userRepo, ITokenGenerator tokenGen)
        {
            _userRepo = userRepo;
            _tokenGen = tokenGen;
        }

        public async Task<GenericResponseDTO<AuthResponseDTO>> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(request.Email);
                if(user == null)
                {
                    return new GenericResponseDTO<AuthResponseDTO>
                    {
                        isSuccess = false,
                        message = "Email or Password invalid."
                    };
                }

                var verifiedPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.HashPass);
                if(!verifiedPassword)
                {
                    return new GenericResponseDTO<AuthResponseDTO>
                    {
                        isSuccess = false,
                        message = "Email or Password invalid."
                    };
                }

                var token = _tokenGen.GenerateToken(user);

                return new GenericResponseDTO<AuthResponseDTO>
                {
                    isSuccess = true,
                    message = "Login Successfully",
                    Data = new AuthResponseDTO
                    {
                        UserId = user.userId,
                        Name = user.Name,
                        Email = user.Email,
                        Token = token
                    }
                };
            }
            catch(Exception ex)
            {
                return new GenericResponseDTO<AuthResponseDTO>
                {
                    isSuccess = false,
                    message = ex.Message
                };
            }
        }
    }
}
