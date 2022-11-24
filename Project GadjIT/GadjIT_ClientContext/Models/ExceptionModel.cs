using System.Collections.Generic;

namespace GadjIT_ClientContext.Models
{
    public class ExceptionModel
    {
        public int StatusCode {get; set;}
        public string Method { get; set; } = "";
        public string Message { get; set; } = "";
        public string StackTrace { get; set; } = "";
    
    }
}