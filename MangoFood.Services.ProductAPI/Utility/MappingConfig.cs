using AutoMapper;
using MangoFood.Services.ProductAPI.Models;
using MangoFood.Services.ProductAPI.Models.DTO;

namespace MangoFood.Services.ProductAPI.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(options =>
            {
                options.CreateMap<ProductDTO, Product>();
                options.CreateMap<Product, ProductDTO>();
            });

            return mappingConfig;
        }
    }
}
