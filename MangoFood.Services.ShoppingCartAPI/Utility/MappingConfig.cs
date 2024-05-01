using AutoMapper;
using MangoFood.Services.ShoppingCartAPI.Models;
using MangoFood.Services.ShoppingCartAPI.Models.DTO;


namespace MangoFood.Services.ShoppingCartAPI.Utility
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(options =>
            {
                options.CreateMap<ShoppingCartDetails, ShoppingCartDetailsDTO>();
                options.CreateMap<ShoppingCartHeader, ShoppingCartHeaderDTO>();
                options.CreateMap<ShoppingCartHeaderDTO, ShoppingCartHeader>();
                options.CreateMap<ShoppingCartDetailsDTO, ShoppingCartDetails>();
            });

            return mappingConfig;
        }
    }
}
