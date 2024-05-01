using AutoMapper;
using MangoFood.Services.CouponAPI.Models;
using MangoFood.Services.CouponAPI.Models.DTO;

namespace MangoFood.Services.CouponAPI.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(options =>
            {
                options.CreateMap<CouponDTO, Coupon>();
                options.CreateMap<Coupon, CouponDTO>();
            });

            return mappingConfig;
        }
    }
}
