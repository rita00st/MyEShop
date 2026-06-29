using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel.ProductGroup;

namespace MyEShop.Models.Services.Interface
{
    public interface IGroupProductService
    {
        public IEnumerable<Category> AllCategory();
        public IEnumerable<ProductGroupViewModel> ShowCategory();
    }
}
