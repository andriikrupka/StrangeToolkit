namespace StrangeToolkit.Utils
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T somObject) where T : class
        {
            return somObject == null;
        }

        public static bool IsNotNull<T>(this T somObject) where T : class
        {
            return somObject != null;
        }
    }
}
