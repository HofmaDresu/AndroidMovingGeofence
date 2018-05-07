using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Location;
using Android.Content;
using Android.Gms.Tasks;

namespace MovingGeofence
{
    [Activity(Label = "MovingGeofence", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, IOnCompleteListener
    {
        int count = 1;

		public void OnComplete(Task task)
		{
			var foo = 1;
		}

		protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = $"{count++} clicks!"; };

			var locationClient = LocationServices.GetFusedLocationProviderClient(this);


			var geofencingClient = LocationServices.GetGeofencingClient(this);

			var lastLocation = await locationClient.GetLastLocationAsync();

			var geoFence = new GeofenceBuilder()
				.SetRequestId("fence")
				.SetCircularRegion(lastLocation.Latitude, lastLocation.Longitude, 100)
				.SetTransitionTypes(Geofence.GeofenceTransitionExit | Geofence.GeofenceTransitionEnter)
				.SetExpirationDuration(Geofence.NeverExpire)
				.Build();

			var geofenceRequest = new GeofencingRequest.Builder()
													   .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
													   .AddGeofence(geoFence)
													   .Build();

			var geoIntent = new Intent(this, typeof(GeofenceTransitionsService));
			var pendingGeoIntent = PendingIntent.GetService(this, 0, geoIntent, PendingIntentFlags.UpdateCurrent);

			geofencingClient.AddGeofences(geofenceRequest, pendingGeoIntent).AddOnCompleteListener(this);
        }
    }
}

