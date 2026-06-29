using MyEShop.Models.Entities;

namespace MyEShop.Models.Services.Interface
{
    public interface IUserService
    {
        public void AddUser(User user);
        public User GetUserForLogin(string email , string password);
        public bool IsExistUserByEmail(string email);
    }
}
