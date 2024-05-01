using AutoMapper;
using MangoFood.Services.OrderAPI.Models;
using MangoFood.Services.OrderAPI.Models.DTO;



namespace MangoFood.Services.OrderAPI.Utility
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(options =>
            {
                options.CreateMap<OrderHeaderDTO, ShoppingCartHeaderDTO>().ReverseMap();

                options.CreateMap<ShoppingCartDetailsDTO, OrderDetailsDTO>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, u => u.MapFrom(src => src.Product.Price));

                options.CreateMap<OrderDetailsDTO, ShoppingCartDetailsDTO>();

                options.CreateMap<OrderHeaderDTO, OrderHeader>().ReverseMap();
                options.CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
