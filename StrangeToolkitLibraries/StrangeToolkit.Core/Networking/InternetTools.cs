namespace StrangeToolkit.Networking
{
    using StrangeToolkit.Utils;
    using System;
    using System.Linq;
    using Windows.Networking.Connectivity;

    public static class InternetTools
    {
        private static readonly int[] availableIanaNetworkInterfaces =
        {
            6, //ethernet
            71, //wifi
            216, //GTP (GPRS Tunneling Protocol)
            243, //3g
            244 //3GPP2 WWA
        };

        private static bool? isConnected;

        public static event EventHandler<InternetConnectionChangedEventArgs> InternetConnectionChanged;

        static InternetTools()
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkInformationNetworkStatusChanged;
        }

        private static void OnNetworkInformationNetworkStatusChanged(object sender)
        {
            var args = new InternetConnectionChangedEventArgs { IsConnected = InternetTools.CheckInternetConnection() };
            InternetTools.isConnected = args.IsConnected;
            InternetConnectionChanged.SafeInvokeEvent<InternetConnectionChangedEventArgs>(sender, args);
        }

        public static bool IsConnected
        {
            get
            {
                if (!InternetTools.isConnected.HasValue)
                {
                    InternetTools.isConnected = InternetTools.CheckInternetConnection();
                }

                return InternetTools.isConnected.Value;
            }
        }

        /// <summary>
        /// Static method for check internet connection.
        /// </summary>
        /// <returns></returns>
        private static bool CheckInternetConnection()
        {
            var internetConnection = false;

            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                var networkConnectivityLevel = connectionProfile.GetNetworkConnectivityLevel();
                var isIanaAvailable = availableIanaNetworkInterfaces.Any(a => a == connectionProfile.NetworkAdapter.IanaInterfaceType);
                if (connectionProfile.NetworkAdapter.InboundMaxBitsPerSecond > 0 && isIanaAvailable && networkConnectivityLevel == NetworkConnectivityLevel.InternetAccess)
                {
                    internetConnection = true;
                }
            }

            return internetConnection;
        }
    }
}
