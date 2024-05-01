using MangoFood.Web.Utility;
using static MangoFood.Web.Utility.SD;

namespace MangoFood.Web.Models.DTO
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public SD.ConentType ConentType { get; set; } = SD.ConentType.Json;
    }
}
