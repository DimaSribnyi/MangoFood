using MangoFood.Web.Models.DTO;
using MangoFood.Web.Services.IService;
using Newtonsoft.Json;
using System.Text;
using static MangoFood.Web.Utility.SD;

namespace MangoFood.Web.Services.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("MangoFoodAPI");

                HttpRequestMessage requestMessage = new();
                if(requestDTO.ConentType == ConentType.MultipartFormData)
                {
                    requestMessage.Headers.Add("Accept", "*/*"); // Any media type
                }
                else
                {
                    requestMessage.Headers.Add("Accept", "application/json");
                }
                //TODO: token
                if(withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    requestMessage.Headers.Add("Authorization", $"Bearer {token}");
                }

                requestMessage.RequestUri = new Uri(requestDTO.Url);

                if(requestDTO.ConentType == ConentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();
                    foreach(var prop in requestDTO.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDTO.Data);
                        if(value is FormFile)
                        {
                            var file = (FormFile)value;
                            if(file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
                    }
                    requestMessage.Content = content;   
                }
                else
                {
                    if (requestDTO.Data != null)
                    {
                        requestMessage.Content = new StringContent(JsonConvert
                            .SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                    }
                }

                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        requestMessage.Method = HttpMethod.Post;
                        break;

                    case ApiType.PUT:
                        requestMessage.Method = HttpMethod.Put;
                        break;

                    case ApiType.DELETE:
                        requestMessage.Method = HttpMethod.Delete;
                        break;

                    default:
                        requestMessage.Method = HttpMethod.Get;
                        break;
                };

                var responseMessage = await httpClient.SendAsync(requestMessage);

                switch (responseMessage.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        {
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            var errorResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(errorContent);
                            return errorResponseDTO ?? new() { Success = false, Message = "Not Found" };
                        }

                    case System.Net.HttpStatusCode.Forbidden:
                        {
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            var errorResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(errorContent);
                            return errorResponseDTO ?? new() { Success = false, Message = "Access Denied" };
                        }

                    case System.Net.HttpStatusCode.Unauthorized:
                        {
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            var errorResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(errorContent);
                            return errorResponseDTO ?? new() { Success = false, Message = "Unauthorized" };
                        }

                    case System.Net.HttpStatusCode.InternalServerError:
                        {
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            var errorResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(errorContent);
                            return errorResponseDTO ?? new() { Success = false, Message = "Internal Server Error" };
                        }

                    case System.Net.HttpStatusCode.BadRequest:
                        {
                            var errorContent = await responseMessage.Content.ReadAsStringAsync();
                            var errorResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(errorContent);
                            return errorResponseDTO ?? new() { Success = false, Message = "Bad Request" };
                        }

                    default:
                        var apiContent = await responseMessage.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            } catch(Exception ex)
            {
				if (ex.InnerException != null)
				{
					return new() { Success = false, Message = ex.InnerException.Message };
				}
				return new() { Success = false, Message = ex.Message };
			}
        }
    }
}
