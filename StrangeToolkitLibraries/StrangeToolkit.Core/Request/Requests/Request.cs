namespace StrangeToolkit.Request
{
    using System;
    using System.Collections.Generic;
    using Utils;

    public abstract class Request
    {
        internal readonly ParametersCollection parameters;

        public RequestCookie Cookies { get; set; }

        public HttpMethodType HttpMethod { get; set; }

        public string Address { get; set; }

        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SetCookieHeader { get; protected set; }

        public bool TryGetSetCookieHeader { get; set; }

        protected Request()
        {
            this.Headers = new Dictionary<string, string>();
            this.parameters = new ParametersCollection();
            this.parameters.Add("noCacheGuidKey", Guid.NewGuid().ToString());
        }

        protected Request(HttpMethodType httpMethod)
            : this()
        {
            Guard.ArgumentNotNull(httpMethod, "httpMethod");

            this.HttpMethod = httpMethod;
        }

        protected Request(HttpMethodType httpMethod, string address)
            : this(httpMethod)
        {
            Guard.ArgumentNotNullOrEmptyString(address, "address");

            this.Address = address;
        }

        /// <summary>
        /// The value of the User-agent HTTP header. 
        /// Uses TryAdd.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Determines which HttpClientHandler to use: System.Net or Windows.Web include IHttpFilter with Ignore cetificates errors.
        /// </summary>
        public bool IgnoreServerCertificateErrors { get; set; }

        /// <summary>
        /// Try get cookie from WebView Application instance.
        /// </summary>
        public bool TryUseWebViewCookie { get; set; }

        public void AddParameter(string name, string value)
        {
            this.parameters.Add(name, value);
        }

        public void AddParameter(string name, double value)
        {
            this.parameters.Add(name, value.ToString());
        }
    }
}
