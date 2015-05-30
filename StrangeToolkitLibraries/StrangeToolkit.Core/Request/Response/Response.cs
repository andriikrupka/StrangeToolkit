namespace StrangeToolkit.Request
{
    public class Response<T>
    {
        public T Result { get; set; }

        public RequestError Error { get; set; }

        public bool HasError
        {
            get
            {
                return this.Error != null;
            }
        }

        public bool IsSuccess
        {
            get
            {
                return this.Error == null && this.Result != null;
            }
        }
    }
}
