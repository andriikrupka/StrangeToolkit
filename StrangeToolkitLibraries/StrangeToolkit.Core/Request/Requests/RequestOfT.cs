using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Minder.DataAccess.Core.WindowsRuntime.HttpClientFilters;
using Newtonsoft.Json;
using StrangeToolkit.Utils;

namespace StrangeToolkit.Request
{
    public class Request<T> : Request
    {
        protected readonly Parser<T> parser;

        public Request(Parser<T> parser)
            : base()
        {
            Guard.ArgumentNotNull(parser, "parser");
            this.parser = parser;
        }

        public Request(Parser<T> parser, HttpMethodType httpMethod)
            : base(httpMethod)
        {
            Guard.ArgumentNotNull(parser, "parser");
            this.parser = parser;
        }

        public Request(Parser<T> parser, HttpMethodType httpMethod, string address)
            : base(httpMethod, address)
        {
            Guard.ArgumentNotNull(parser, "parser");
            this.parser = parser;
        }

        public object PostContent { get; set; }

        public string JsonStringContent { get; set; }

        protected virtual string BuildParametersData()
        {
            var linkBuilder = new StringBuilder();
            if (this.parameters.Any())
            {
                if (this.HttpMethod == HttpMethodType.Get)
                {
                    if (!this.Address.Contains("?"))
                    {
                        var lastLinkSymbol = this.Address[this.Address.Length - 1];

                        if (lastLinkSymbol == '/')
                        {
                            this.Address = this.Address.Substring(0, this.Address.Length - 1);
                        }

                        linkBuilder.Append("?");
                    }
                    else
                    {
                        linkBuilder.Append("&");
                    }
                }

                foreach (var parameter in this.parameters)
                {
                    Guard.ArgumentNotNullOrEmptyString(parameter.Key, "parameter.Key");
                    Guard.ArgumentNotNullOrEmptyString(parameter.Value, "parameter.Value");

                    linkBuilder.AppendFormat(CultureInfo.InvariantCulture, parameter.Key + "=" + Uri.EscapeDataString(parameter.Value) + "&");
                }

                linkBuilder.Remove(linkBuilder.Length - 1, 1);
            }

            return linkBuilder.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public virtual async Task<Response<T>> SendRequestAsync()
        {
            var responseData = new Response<T>();

            using (var httpRequestMessage = new System.Net.Http.HttpRequestMessage())
            {
                using (var filter = new HttpBaseProtocolFilter())
                {
                    if (this.TryUseWebViewCookie)
                    {
                        var cookieManager = filter.CookieManager;
                        var cookies = cookieManager.GetCookies(new Uri(this.Address));
                        if (cookies.Count > 0)
                        {
                            var cookieCollection = new CookieCollection();
                            foreach (var cookie in cookies)
                            {
                                cookieCollection.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path));
                            }

                            this.Cookies = new RequestCookie(cookieCollection);
                        }
                    }

                    using (var handler = this.CreateHttpClientHandler(filter))
                    {
                        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                        if (this.Cookies != null)
                        {
                            handler.UseCookies = true;
                            var сookieContainer = new CookieContainer();
                            сookieContainer.Add(new Uri(this.Address), this.Cookies.GetCookieCollection());
                            handler.CookieContainer = сookieContainer;
                        }

                        using (var httpClient = new System.Net.Http.HttpClient(handler))
                        {
                            if (!string.IsNullOrEmpty(this.UserAgent))
                            {
                                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(this.UserAgent);
                            }

                            foreach (var header in this.Headers)
                            {
                                httpRequestMessage.Headers.Add(header.Key, header.Value);
                            }

                            httpClient.Timeout = TimeSpan.FromSeconds(60);

                            var parametersData = this.BuildParametersData();

                            httpRequestMessage.Method = HttpMethodUtils.ConvertToHttpMethod(this.HttpMethod);
                            switch (this.HttpMethod)
                            {
                                case HttpMethodType.Get:
                                    httpRequestMessage.RequestUri = new Uri(this.Address + parametersData);
                                    break;

                                case HttpMethodType.Post:
                                case HttpMethodType.Put:
                                    if (this.PostContent.IsNull())
                                    {
                                        if (!string.IsNullOrEmpty(parametersData))
                                        {
                                            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                                            httpRequestMessage.Content = new StringContent(parametersData);
                                        }
                                    }
                                    else
                                    {
                                        var jsonContent = this.JsonStringContent;
                                        if (string.IsNullOrEmpty(this.JsonStringContent))
                                        {
                                            jsonContent = this.PreparePostContentData();
                                        }

                                        httpRequestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                                    }

                                    httpRequestMessage.RequestUri = new Uri(this.Address);

                                    break;

                                default:
                                    throw new NotSupportedException(string.Format("httpMethod {0}", this.HttpMethod));
                                    break;
                            }

                            try
                            {
                                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                                if (httpResponseMessage != null)
                                {
                                    using (httpResponseMessage)
                                    {
                                        if (httpResponseMessage.IsSuccessStatusCode)
                                        {
                                            if (this.TryGetSetCookieHeader)
                                            {
                                                this.ReadHeaderSetCookie(httpResponseMessage.Headers);    
                                            }

                                            if (httpResponseMessage.Content != null)
                                            {
                                                if (httpResponseMessage.Content.Headers.ContentType.CharSet == "utf-8")
                                                {
                                                    var @string = await httpResponseMessage.Content.ReadAsStringAsync();
                                                    responseData = this.parser.Parse(@string, this.Cookies);
                                                }
                                                else
                                                {
                                                    var byteArray = await httpResponseMessage.Content.ReadAsStreamAsync();
                                                    var streamReader = new StreamReader(byteArray, Encoding.UTF8);
                                                    var @string = streamReader.ReadToEnd();
                                                    responseData = this.parser.Parse(@string, this.Cookies);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var error = new RequestError
                                            {
                                                ErrorType = ErrorType.Server,
                                                ErrorCode = (int)httpResponseMessage.StatusCode
                                            };

                                            if (httpResponseMessage.Content != null)
                                            {
                                                var contentData = await httpResponseMessage.Content.ReadAsStringAsync();
                                                this.TryParseError(contentData, error);
                                            }

                                            responseData.Error = error;
                                        }
                                    }
                                }
                                else
                                {
                                    responseData.Error = new RequestError { ErrorType = ErrorType.Unknown };
                                }
                            }
                            catch (TaskCanceledException ex)
                            {
                                responseData.Error = new RequestError
                                {
                                    ErrorType = ErrorType.TaskCanceled,
                                    Message = ex.Message
                                };
                            }
                            catch (HttpRequestException ex)
                            {
                                responseData.Error = new RequestError
                                {
                                    ErrorType = ErrorType.Unknown,
                                    Message = ex.Message
                                };
                            }
                            catch (Exception ex)
                            {
                                responseData.Error = new RequestError
                                {
                                    ErrorType = ErrorType.Unknown,
                                    Message = ex.Message
                                };
                            }
                        }
                    }
                }
            }

            return responseData;
        }

        private void ReadHeaderSetCookie(HttpHeaders httpResponseHeaders)
        {
            IEnumerable<string> values;
            if (httpResponseHeaders.TryGetValues("set-cookie", out values))
            {
                this.SetCookieHeader = string.Join(string.Empty, values);
            }
        }

        protected virtual void TryParseError(string contentData, RequestError error)
        {

        }

        private string PreparePostContentData()
        {
            var postContentString = JsonConvert.SerializeObject(this.PostContent);
            return postContentString;
        }

        private HttpClientHandler CreateHttpClientHandler(HttpBaseProtocolFilter httpFilter)
        {
            HttpClientHandler httpClientHandler;
            if (this.IgnoreServerCertificateErrors)
            {
                httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
                httpClientHandler = new WinRtHttpClientHandler(httpFilter);
            }
            else
            {
                httpClientHandler = new HttpClientHandler();
            }

            return httpClientHandler;
        }
    }
}
