using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users?> GetUserByEmailAsync(string email);
        Task AddUserAsync(Users user);
        Task SaveChangesAsync();

    }
}
