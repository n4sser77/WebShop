using WebShop.Data;

namespace WebShop.Models.Interfaces
{
    public interface IAdminManager
    {

        public Task AddAdmin(User user);
        public Task RemoveAdmin(User user);

    }
}

