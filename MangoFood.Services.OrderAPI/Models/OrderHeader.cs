﻿using System.ComponentModel.DataAnnotations;

namespace MangoFood.Services.OrderAPI.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentInentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
