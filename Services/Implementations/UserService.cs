using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly StoreDbContext storeDbContext;

        public UserService(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<List<User>> FindAll()
        {
            return await storeDbContext.Users.ToListAsync();
        }
    }
}
