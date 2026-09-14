using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        public IList<User> Users { get; set; } = new List<User>();

        // آمار
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int NormalUserCount { get; set; }
        public int TodayRegistrations { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _context.users
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            TotalUsers = Users.Count;
            AdminCount = Users.Count(u => u.IsAdmin);
            NormalUserCount = Users.Count(u => !u.IsAdmin);
            TodayRegistrations = Users.Count(u => u.RejesterDate.Date == DateTime.Today);
        }
    }
}