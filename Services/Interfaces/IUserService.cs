using WebBanQuanAo.Models;

namespace WebBanQuanAo.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> FindAll();
    }
}
