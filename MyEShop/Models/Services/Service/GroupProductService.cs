using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.ViewModel.ProductGroup;

namespace MyEShop.Models.Services.Service
{
    public class GroupProductService : IGroupProductService
    {
        private MyEshopContext _context;
        public GroupProductService(MyEshopContext context)
        {
            _context = context;
        }

        

        public IEnumerable<Category> AllCategory ()
        {
            return _context.Categories;
        }
        public IEnumerable<ProductGroupViewModel> ShowCategory ()
        { 
            return _context.Categories.Select(c => new ProductGroupViewModel()
            {
                GroupId = c.Id,
                Name = c.Name,
                ProductCount = _context.CategoryToProducts.Count(g => g.CategoryId == c.Id)
            }).ToList();
        }
    }
}
