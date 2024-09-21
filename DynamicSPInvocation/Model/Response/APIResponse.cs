using System.Net;
using System.Text.Json.Serialization;

namespace DynamicSPInvocation.Model.Response
{
    public class APIResponse<T>
    {
        public T Data { get; set; }

        public HttpStatusCode statusCode { get; set; }

        //[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string statusMessage { get; set; }
    }
}