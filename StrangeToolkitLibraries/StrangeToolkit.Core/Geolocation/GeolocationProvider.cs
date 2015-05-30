namespace StrangeToolkit.Geolocation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;

    public sealed class GeolocationProvider
    {
        #region Implementation of Singletone

        private static readonly Lazy<GeolocationProvider> instanceLazy = new Lazy<GeolocationProvider>(() => new GeolocationProvider(), true);

        public static GeolocationProvider Instance
        {
            get { return instanceLazy.Value; }
        }

        private GeolocationProvider()
        {
        }

        #endregion

        public Task<GeolocationResponse> GetGeolocatorResponseAsync()
        {
            return this.GetGeolocatorResponseAsync(CancellationToken.None);
        }

        public async Task<GeolocationResponse> GetGeolocatorResponseAsync(CancellationToken cancellationToken)
        {
            var response = new GeolocationResponse();

            var state = GeolocationState.Active;
            var geolocator = new Geolocator();

            try
            {
                var result = await geolocator.GetGeopositionAsync().AsTask(cancellationToken);
                response.CurrentGeoposition = result;
                response.Geolocator = geolocator;
            }
            catch (TaskCanceledException)
            {
                state = GeolocationState.TaskCanceled;
            }
            catch (Exception)
            {
                state = GeolocationState.Error;
                if (geolocator.LocationStatus == PositionStatus.Disabled)
                {
                    state = GeolocationState.Disabled;
                }
            }

            response.State = state;

            return response;
        }
    }
}
