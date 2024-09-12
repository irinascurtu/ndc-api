using AutoMapper;
using ProductsApi.Data.Entities;
using ProductsApi.Models;

namespace ProductsApi.Infrastructure.Mappings
{
    public class ProductProfileMapping : Profile
    {
        public ProductProfileMapping()
        {
            CreateMap<Product, ProductModel>();
            CreateMap<ProductModel, Product>();
        }
    }
}
