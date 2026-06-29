using Microsoft.AspNetCore.Http.HttpResults;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.Services.Interface;

namespace MyEShop.Models.Services.Service
{
    public class UserService : IUserService
    {
        private MyEshopContext _context;
        public UserService(MyEshopContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public User GetUserForLogin(string email, string password)
        {
            var user = _context.users.SingleOrDefault(u=>u.Email ==  email && u.Password==password);
            //if(user == null)
            //{
            //    throw new KeyNotFoundException("ایمیل یا رمز عبور ببه درستی وارد نشده");
            //}
            return user;
        }

        public bool IsExistUserByEmail(string email)
        {
            return _context.users.Any(u => u.Email == email);
        }
    }
}
