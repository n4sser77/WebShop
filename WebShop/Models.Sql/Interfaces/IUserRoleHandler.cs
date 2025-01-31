namespace WebShop.Models.Sql.Interfaces
{

    public interface IUserRoleHandler
    {
        
        Task<bool> HandleUserSessionAsync(User user);
    }
}