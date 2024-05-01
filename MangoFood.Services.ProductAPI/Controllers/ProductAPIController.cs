using AutoMapper;
using Azure;
using MangoFood.Services.ProductAPI.Data;
using MangoFood.Services.ProductAPI.Models;
using MangoFood.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangoFood.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDTO _response;

        public ProductAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetProducts()
        {
            try
            {
                var products = await _context.Products.AsNoTracking().ToListAsync();
                _response.Result = _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDTO>> GetProductByID(int id)
        {
            try
            {
                var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);

                if(product == null)
                {
                    _response.Success = false;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> PostProduct(ProductDTO productDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(productDTO);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if(productDTO.Image != null)
                {
                    var fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    var filePath = @"wwwroot\ProductImages\" + fileName;
                    var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using(var fileStream = new FileStream(fileDirectory, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://" +
                        $"{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _response.Result = _mapper.Map<ProductDTO>( product);    
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> PutProduct(ProductDTO productDTO, int id)
        {
            if(id != productDTO.ProductId)
            {
                _response.Success = false;
                return BadRequest(_response);
            }
            try
            {
                var product = _mapper.Map<Product>(productDTO);


                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo fileInfo = new(oldPath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }

                if (productDTO.Image != null)
                {
                    var fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    var filePath = @"wwwroot\ProductImages\" + fileName;
                    var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(fileDirectory, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://" +
                        $"{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CatchEx(ex);
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                if(product == null)
                {
                    _response.Success = false;
                    return NotFound(_response);
                }

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo fileInfo = new(oldPath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }

                _context.Remove(product);
                await _context.SaveChangesAsync();
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
