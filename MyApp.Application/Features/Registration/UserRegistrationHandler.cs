using MediatR;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces.Repository;
using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Features.Registration
{
    public class UserRegistrationHandler : IRequestHandler<UserRegistrationCommand, GenericResponseDTO<UserRegistrationResponseDTO>>
    {
        private readonly IUserRepository _userRepo;

        public UserRegistrationHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<GenericResponseDTO<UserRegistrationResponseDTO>> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);


                var user = new Users(request.Name, request.Email, hashPassword);

                await _userRepo.AddUserAsync(user);
                await _userRepo.SaveChangesAsync();

                return new GenericResponseDTO<UserRegistrationResponseDTO>
                {
                    isSuccess = true,
                    message = "User Registered Successfully.",
                    Data = new UserRegistrationResponseDTO
                    {
                        UserId = user.userId,
                        Name = user.Name,
                        Email = user.Email
                    }
                };

            }
            catch(Exception ex)
            {
                return new GenericResponseDTO<UserRegistrationResponseDTO>
                {
                    isSuccess = false,
                    message = ex.Message
                };
            }
        }
    }
}
