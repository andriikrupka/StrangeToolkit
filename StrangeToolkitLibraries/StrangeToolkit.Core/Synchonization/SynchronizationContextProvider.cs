namespace StrangeToolkit.Synchonization
{
    using System.Threading;

    public static class SynchronizationContextProvider
    {
        public static void Initialize()
        {
            UIThreadContext = SynchronizationContext.Current;
        }

        public static SynchronizationContext UIThreadContext { get; private set; }
    }
}
