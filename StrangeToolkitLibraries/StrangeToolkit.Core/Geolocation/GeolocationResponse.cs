using Windows.Devices.Geolocation;

namespace StrangeToolkit.Geolocation
{
    public sealed class GeolocationResponse
    {
        public Geolocator Geolocator { get; internal set; }

        public Geoposition CurrentGeoposition { get; internal set; }

        public GeolocationState State { get; internal set; }

        public bool IsSuccess
        {
            get { return this.State == GeolocationState.Active; }
        }

        internal GeolocationResponse()
        {
            
        }
    }
}
