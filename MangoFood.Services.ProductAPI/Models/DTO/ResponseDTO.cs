﻿
namespace MangoFood.Services.ProductAPI.Models.DTO
{
    public class ResponseDTO
    {
        public Object? Result { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
    }
}
