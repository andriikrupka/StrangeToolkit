namespace StrangeToolkit.Request
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public abstract class Parser<T>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "async pattern")]
        public virtual Response<T> Parse(string data, RequestCookie cookies)
        {
            var result = new Response<T>();

            try
            {
                var parsedResult = this.ParseResponseWithoutErrorHandling(data, cookies);
                result = parsedResult;
            }
            catch (Exception ex)
            {
                result.Error = new RequestError
                {
                    ErrorType = ErrorType.Parsing,
                    Message = ex.Message
                };
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "does not meet the business logic of the application")]
        protected virtual Response<T> ParseResponseWithoutErrorHandling(string data, RequestCookie cookies)
        {
            var result = new Response<T>
            {
                Result = this.ParseWithoutErrorHandling(data, cookies)
            };

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Async pattern")]
        public virtual Response<T> Parse(string data, RequestCookie cookies, string link, bool isUseResponseOfT)
        {
            return this.Parse(data, cookies);
        }

        protected virtual T ParseWithoutErrorHandling(string data, RequestCookie cookies)
        {
            return default(T);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "does not meet the business logic of the application")]
        public virtual Task<Response<T>> ParseErrorResponse(string data)
        {
            throw new NotSupportedException();
        }
    }
}
