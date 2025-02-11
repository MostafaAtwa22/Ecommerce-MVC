using AutoMapper;
using Ecommerce.Entities.ViewModels.Products;

namespace Ecommerce.Web.helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            Category();
            Product();
        }
        private void Category ()
        {
            CreateCategory();
            EditCategory();
        }
        private void CreateCategory()
        {
            CreateMap<Category, CategoryVM>()
                .ReverseMap();
        }
        private void EditCategory()
        {
            CreateMap<Category, EditCategoryVM>()
                .ReverseMap();
        }

        private void Product()
        {
            CreateProduct();
            EditProduct();
        }
        private void CreateProduct()
        {
            CreateMap<Product, CreateProductVM>()
                .ForMember(dest => dest.Quantity, src => src.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ReverseMap();
        }
        private void EditProduct()
        {
            CreateMap<Product, EditProductVM>()
                .ForMember(dest => dest.Quantity, src => src.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CurrentImage, src => src.MapFrom(src => src.Image))
                .ForMember(dest => dest.CategoriesList, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore()) 
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }

    }
}
