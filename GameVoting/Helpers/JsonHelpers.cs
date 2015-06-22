using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GameVoting.Helpers
{
    public static class JsonHelpers
    {
        private class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Payload { get; set; }
        }

        public static string ErrorResponse(Exception e)
        {
            StringBuilder messageBuilder = new StringBuilder();

            while (e != null)
            {
                messageBuilder.AppendLine(e.Message);
                e = e.InnerException;
            }

            return JsonConvert.SerializeObject(new Response { Success = false, Message = messageBuilder.ToString() });
        }

        public static string ErrorResponse(string message)
        {
            return JsonConvert.SerializeObject(new Response { Success = false, Message = message });
        }

        public static string SuccessResponse(string message)
        {
            return JsonConvert.SerializeObject(new Response { Success = true, Message = message });
        }

        public static string SuccessResponse(string message, object payload)
        {
            return JsonConvert.SerializeObject(new Response { Success = true, Message = message, Payload = payload });
        }
    }
}