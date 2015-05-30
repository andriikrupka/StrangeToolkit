namespace StrangeToolkit.Models
{
    using System;
    using System.Reflection;

    public class WeakEventHandler<TEventArgs> 
    {
        private readonly WeakReference targetReference;
        private readonly MethodInfo method;

        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            this.method = callback.GetMethodInfo();
            this.targetReference = new WeakReference(callback.Target, true);
        }

        public void Handler(object sender, TEventArgs args)
        {
            var target = this.targetReference.Target;

            if (target != null)
            {
                var parameters = new object[2] { sender, args };
                this.method.Invoke(target, parameters);
            }
        }
    }
}
