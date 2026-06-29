using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class CreateModel : PageModel
    {
        private readonly MyEShop.Models.DatabaseContext.MyEshopContext _context;

        public CreateModel(MyEShop.Models.DatabaseContext.MyEshopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
