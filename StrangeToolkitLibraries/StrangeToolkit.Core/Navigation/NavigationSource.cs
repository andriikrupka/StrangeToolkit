namespace StrangeToolkit.Navigation
{
    using System.Runtime.Serialization;

    [DataContract]
    public class NavigationSource
    {
        [DataMember]
        public object AssociatedSource { get; set; }

        [DataMember]
        public INavigationParameters Parameters { get; set; }

        public object Page { get; set; }
    }
}
