using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;


namespace MyEShop.Controllers
{
    public class ProductController : Controller
    {
        private MyEshopContext _context;

        public ProductController(MyEshopContext context)
        {
            _context = context;
        }

        [Route("Group/{id}/{name}")]
        public IActionResult ShowProductByGroupId(int id,string name)
        {
            ViewData["GroupName"] = name;

            var product = _context.CategoryToProducts
                .Where(c=>c.CategoryId == id)
                .Include(p=>p.Product)
                .Select(p=>p.Product)
                .ToList();

            return View(product);
        }


        

    }
}
