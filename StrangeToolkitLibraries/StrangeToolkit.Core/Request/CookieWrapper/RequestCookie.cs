using System;
using System.Net;

namespace StrangeToolkit.Request
{
    public sealed class RequestCookie
    {
        private readonly Uri DefaultUri = new Uri("http://127.0.0.1/");

        private readonly CookieContainer cookieContainer = new CookieContainer();

        private readonly CookieCollection cookieCollection = new CookieCollection();

        public RequestCookie(string cookieHeader)
        {
            this.cookieCollection.Add(this.GetCookieFromString(cookieHeader));
        }

        public RequestCookie(CookieCollection inputCookieCollection)
        {
            this.cookieCollection.Add(inputCookieCollection);
        }

        public CookieCollection GetCookieCollection()
        {
            return this.cookieCollection;
        }

        internal CookieCollection GetCookieFromString(string cookieHeader)
        {
            this.cookieContainer.SetCookies(this.DefaultUri, cookieHeader);
            return this.cookieContainer.GetCookies(this.DefaultUri);
        }
    }
}
