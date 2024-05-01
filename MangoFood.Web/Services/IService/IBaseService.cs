using MangoFood.Web.Models.DTO;

namespace MangoFood.Web.Services.IService
{
    public interface IBaseService
    {
        /// <summary>
        /// Sends a request to an API
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
    }
}
