namespace StrangeToolkit.Request
{
    public class RequestError
    {
        public ErrorType ErrorType { get; set; }

        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public object AssociatedObject { get; set; }
    }
}
