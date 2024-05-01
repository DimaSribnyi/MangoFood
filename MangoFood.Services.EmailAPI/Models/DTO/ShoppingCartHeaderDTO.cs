﻿namespace MangoFood.Services.EmailAPI.Models.DTO
{
    public class ShoppingCartHeaderDTO
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        public string? FirstName { get; set; }
        public string? LatName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
