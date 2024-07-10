using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Services.Implementations
{
    public class ConnectionService : IConnectionService
    {
        private readonly StoreDbContext storeDbContext;

        public ConnectionService(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task RemoveConnection(string connectionId)
        {
            Connection? connection = await storeDbContext.Connections
                .SingleOrDefaultAsync(conn => conn.ConnectionId.Equals(connectionId));

            if (connection != null)
                storeDbContext.Connections.Remove(connection); 

            await storeDbContext.SaveChangesAsync();
        }
    }
}
