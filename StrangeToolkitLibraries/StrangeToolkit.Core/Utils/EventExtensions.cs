namespace StrangeToolkit.Utils
{
    using System;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void SafeInvokeEvent(this EventHandler @event, object sender, EventArgs args)
        {
            var temp = Volatile.Read(ref @event);

            Guard.ArgumentNotNull(sender, "sender");
            Guard.ArgumentNotNull(args, "args");

            if (temp != null)
            {
                temp(sender, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void SafeInvokeEvent<TEventArgs>(this EventHandler<TEventArgs> @event, object sender, TEventArgs args) where TEventArgs : EventArgs
        {
            var temp = Volatile.Read(ref @event);
            
            if (temp != null)
                temp(sender, args);
        }
    }
}
