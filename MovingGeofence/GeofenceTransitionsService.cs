﻿using System;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Graphics;
using Android.Support.V4.App;

namespace MovingGeofence
{
	[Service]
	public class GeofenceTransitionsService : IntentService
	{
		protected override void OnHandleIntent(Intent intent)
		{
			var ge = GeofencingEvent.FromIntent(intent);

			var transition = ge.GeofenceTransition;

			if (transition == Geofence.GeofenceTransitionExit)
			{
				var notificationIntent = new Intent(ApplicationContext, typeof(MainActivity));

                var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
                stackBuilder.AddNextIntent(notificationIntent);

                var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

                var builder = new NotificationCompat.Builder(this);
                builder.SetSmallIcon(Resource.Mipmap.Icon)
                    .SetColor(Color.Red)
                    .SetContentTitle("Location changed")
                    .SetContentText("Test")
                    .SetContentIntent(notificationPendingIntent);

                builder.SetAutoCancel(true);

                var mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
				mNotificationManager?.Notify(0, builder.Build());

				SetGeofence();
			}
		}

		private async void SetGeofence() 
		{
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

			var geoIntent = new Intent(ApplicationContext, typeof(GeofenceTransitionsService));
            var pendingGeoIntent = PendingIntent.GetService(this, 0, geoIntent, PendingIntentFlags.UpdateCurrent);

			geofencingClient.AddGeofences(geofenceRequest, pendingGeoIntent);
		}
	}
}
