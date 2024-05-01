using AutoMapper;
using MangoFood.Services.CouponAPI.Data;
using MangoFood.Services.CouponAPI.Models;
using MangoFood.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MangoFood.Services.CouponAPI.Controllers
{
    [Route("api/Coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDTO _response;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _response = new ResponseDTO();
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetCoupons()
        {
            try
            {
                IEnumerable<Coupon> coupons = await _appDbContext.Coupons.ToListAsync();
                _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
                
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseDTO>> GetCouponByID(Guid id)
        {
            try
            {
                Coupon? coupon = await _appDbContext.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CouponId == id);

                if(coupon == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            
            return _response;
        }

        [HttpGet("{couponCode}")]
        public async Task<ActionResult<ResponseDTO>> GetCouponByCode(string couponCode)
        {
            try
            {
                var coupon = await _appDbContext.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());

                if (coupon == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> PostCoupon(CouponDTO couponDTO)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDTO);

                _appDbContext.Coupons.Add(coupon);
                await _appDbContext.SaveChangesAsync();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(coupon.DiscountAmount*100),
                    Name = coupon.CouponCode,
                    Currency = "usd",
                    Id = coupon.CouponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> PutCoupon(Guid id, CouponDTO couponDTO)
        {
            try
            {
                if(id != couponDTO.CouponId)
                {
                    return BadRequest();
                }

                var coupon = _mapper.Map<Coupon>(couponDTO);
                
                _appDbContext.Coupons.Update(coupon);
                await _appDbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                CatchEx(ex);
            }

            return _response;
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> DeleteCoupon(Guid id)
        {
            try
            {
                var coupon = await _appDbContext.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
                if(coupon == null)
                {
                    return NotFound();
                }

                _appDbContext.Remove(coupon);
                await _appDbContext.SaveChangesAsync();

                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);
            }
            catch (Exception ex)
            {
				CatchEx(ex);
			}

            return _response;
        }

        private void CatchEx(Exception ex)
        {
            _response.Success = false;
            if (ex.InnerException != null)
            {
                _response.Message = ex.InnerException.Message;
            }
            else
            {
                _response.Message = ex.Message;
            }
        }
    }
}
