using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly StoreDbContext _dbContext;

        public GroupService(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddToGroup(Group group)
        {
            await _dbContext.Groups.AddAsync(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Group>> FindAllByUsername(string username)
        {
            List<Group> groups = await _dbContext.Groups
                .Include(g  => g.Connections)
                .Where(g => g.GroupName.Contains(username))
                .ToListAsync();

            return groups;
        }

        public async Task<Group> FindGroupByGroupName(string groupName)
        {
            Group? group = await _dbContext.Groups
                .Include(g => g.Connections)
                .SingleOrDefaultAsync(g => g.GroupName.Equals(groupName));
            return group;
        }

        public async Task UpdateGroup(Group group)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
