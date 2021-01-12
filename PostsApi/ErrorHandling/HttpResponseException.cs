using System;

namespace PostsApi.ErrorHandling
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
    }
}
