using Microsoft.AspNetCore.Mvc;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.ViewModel.ProductGroup;
using System.Threading.Tasks;

namespace MyEShop.ViewComponents
{
    public class ProductGroupViewComponent:ViewComponent
    {
        //private MyEshopContext _context;
        private IGroupProductService _groupProductService;

        public ProductGroupViewComponent(IGroupProductService groupProductService)
        {
            _groupProductService = groupProductService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("View", _groupProductService.ShowCategory());
        }
    }
}
