using MangoFood.Web.Models.DTO.OrderDTO;
using MangoFood.Web.Services.IService;
using MangoFood.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MangoFood.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpPost("OrderReadyForPickUp")]
        public async Task<IActionResult> OrderReadyForPickUp(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if(response != null && response.Success)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderIndex));
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            return View();
        }

        [HttpPost("OrderComplete")]
        public async Task<IActionResult> OrderComplete(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.Success)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderIndex));
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            return View();
        }

        [HttpPost("OrderCancel")]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.Success)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderIndex));
            }

            TempData["error"] = response?.Message ?? "Something went wrong";
            return View();
        }

        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = new OrderHeaderDTO();
            var response = await _orderService.GetOrderById(orderId);

            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            if (response != null && response.Success)
            {
                order = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }
            if(!User.IsInRole(SD.RoleAdmin) && userId != order?.UserId)
            {
                return Forbid("Access Denied");
            }
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeaderDTO> orders = new List<OrderHeaderDTO>();
            string userId = string.Empty;
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            var response = await _orderService.GetOrders(userId);
            if(response != null && response.Success)
            {
                orders = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        orders = orders.Where(o => o.Status == SD.Status_Approved);
                        break;
                    case "readyforpickup":
                        orders = orders.Where(o => o.Status == SD.Status_ReadyForPickup);
                        break;
                    case "completed":
                        orders = orders.Where(o => o.Status == SD.Status_Completed);
                        break;
                    case "cancelled":
                        orders = orders.Where(o => o.Status == SD.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }

            return Json(new {data = orders});
        }
    }
}
