using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GadjIT_ClientContext.Models;

namespace GadjIT_App.Services
{
    public static class HttpResponseMessageExtention
    {
        public static async Task<ExceptionModel> ExceptionResponse(this HttpResponseMessage httpResponseMessage)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            ExceptionModel exceptionResponse = JsonConvert.DeserializeObject<ExceptionModel>(responseContent);
            return exceptionResponse;
        }
    }

   
    
}