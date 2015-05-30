namespace StrangeToolkit.Request
{
    using System.Net.Http;

    public class Request<T, TError> : Request<T>
    {
        private readonly ErrorParser<TError> errorParser;

        public Request(Parser<T> parser, ErrorParser<TError> errorParser)
            : base(parser)
        {
            this.errorParser = errorParser;
        }

        public Request(Parser<T> parser, ErrorParser<TError> errorParser, HttpMethodType httpMethod)
            : base(parser, httpMethod)
        {
            this.errorParser = errorParser;
        }

        public Request(Parser<T> parser, ErrorParser<TError> errorParser, HttpMethodType httpMethod, string address)
            : base(parser, httpMethod, address)
        {
            this.errorParser = errorParser;
        }


        protected override void TryParseError(string contentData, RequestError requestError)
        {
            requestError.AssociatedObject = this.errorParser.ParseError(contentData);
        }
    }
}
