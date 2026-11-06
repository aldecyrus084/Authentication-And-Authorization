using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.Repository;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence;

namespace MyApp.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(Users user)
        {
            await _context.User.AddAsync(user);
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _context.User.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
