using AutoMapper;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel.ListProduct;

namespace MyEShop.Mappings  // بهتر است نام فضای نام را تغییر دهید
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
            {
            // مپ کردن Product به DetailsViewModel
            //CreateMap<Product, DetailsViewModel>()
            //    .ForMember(dest => dest.Categories,
            //        opt => opt.MapFrom(src => src.CategoryToProduct != null
            //            ? src.CategoryToProduct.Select(cp => cp.Category).ToList()
            //            : new List<Category>()))
            //    .ForMember(dest => dest.Price,
            //        opt => opt.MapFrom(src => src.Item != null ? src.Item.Price : 0))
            //    .ForMember(dest => dest.QuantityInStock,
            //        opt => opt.MapFrom(src => src.Item != null ? src.Item.QuantityInStock : 0));

            //// اگر نیاز به مپ برعکس دارید (از ViewModel به Entity)
            //CreateMap<DetailsViewModel, Product>()
            //    .ForMember(dest => dest.Item, opt => opt.Ignore())
            //    .ForMember(dest => dest.CategoryToProduct, opt => opt.Ignore());





            CreateMap<Product, DetailsViewModel>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.CategoryToProduct.Select(cp => cp.Category)))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Item != null ? src.Item.Price : 0))
                .ForMember(dest => dest.QuantityInStock,
                    opt => opt.MapFrom(src => src.Item != null ? src.Item.QuantityInStock : 0))
                .ForMember(dest => dest.Product,
                    opt => opt.MapFrom(src => src));


            //CreateMap<Product, DetailsViewModel>()
            //    .ForMember(dest => dest.Categories,
            //        opt => opt.MapFrom(src => src.CategoryToProduct != null
            //            ? src.CategoryToProduct.Select(cp => cp.Category).ToList()
            //            : new List<Category>()))
            //    .ForMember(dest => dest.Price,
            //        opt => opt.MapFrom(src => src.Item != null ? src.Item.Price : 0))
            //    .ForMember(dest => dest.QuantityInStock,
            //        opt => opt.MapFrom(src => src.Item != null ? src.Item.QuantityInStock : 0))
            //    .ForMember(dest => dest.Product,
            //        opt => opt.MapFrom(src => src));

            //CreateMap<Product, DetailsViewModel>();
        }
    }
}