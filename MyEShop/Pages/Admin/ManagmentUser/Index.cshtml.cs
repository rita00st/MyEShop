using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class IndexModel : PageModel
    {
        private readonly MyEShop.Models.DatabaseContext.MyEshopContext _context;

        public IndexModel(MyEShop.Models.DatabaseContext.MyEshopContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.users.ToListAsync();
        }
    }
}
