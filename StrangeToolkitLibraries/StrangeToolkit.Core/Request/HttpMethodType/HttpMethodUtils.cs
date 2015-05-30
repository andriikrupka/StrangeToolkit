namespace StrangeToolkit.Request
{
    using System.Net.Http;

    public static class HttpMethodUtils
    {
        public static HttpMethod ConvertToHttpMethod(HttpMethodType httpMethodType)
        {
            HttpMethod method;
            switch (httpMethodType)
            {
                    case HttpMethodType.Get:
                        method = HttpMethod.Get;
                        break;

                    case HttpMethodType.Post:
                        method = HttpMethod.Post;
                        break;

                    case HttpMethodType.Put:
                        method = HttpMethod.Put;
                        break;

                    case HttpMethodType.Delete:
                        method = HttpMethod.Delete;
                        break;
                    
                default:
                        method = HttpMethod.Get;
                    break;
            }

            return method;
        }
    }
}
