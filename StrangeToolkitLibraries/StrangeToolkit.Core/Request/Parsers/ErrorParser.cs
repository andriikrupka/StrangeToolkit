namespace StrangeToolkit.Request
{
    using System;

    public abstract class ErrorParser<TError>
    {
        public TError ParseError(string data)
        {
            var error = default(TError);

            try
            {
                error = this.ParseResponseWithoutErrorHandling(data);
            }
            catch (Exception ex)
            {
                // ignored
            }

            return error;
        }

        protected abstract TError ParseResponseWithoutErrorHandling(string data);
    }
}
