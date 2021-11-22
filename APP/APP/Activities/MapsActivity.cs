using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Maps;
using Android;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.Support.V4.App;
using System;
using Android.Graphics;
using Java.Util;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using APP.Helpers;
using Android.Content.PM;
using System.Globalization;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MapsActivity : AppCompatActivity, IOnMapReadyCallback
    {
        readonly string[] permissionGroup = { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation };

        TextView fincaVisitaText, fechaText;
        ImageButton locationButton, startButton;
        Button changeMap;
        ImageView volverMaps;
        bool satelite = false;
        int idVisita, idLectura;
        string nombreFinca, nombreVisita, nombreFecha, activity;

        GoogleMap map;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location myLastLocation;
        LatLng myPosition;
        LatLng destinationPoint;
        LatLng startPosition;

        LocationRequest myLocationRequest;
        LocationCallbackHelper myLocationCallback = new LocationCallbackHelper();

        //Brujula
        SensorSpeed speed = SensorSpeed.UI;
        float degrees = 0;

        Marker currentPositionMarker;

        ObservableCollection<ArrayList> coordenadas;
        ObservableCollection<LatLng> puntosLecturas;
        ObservableCollection<string> tipo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            activity = Intent.GetStringExtra("activity");

            if (activity == "RecorridoActivity")
            {
                idVisita = Intent.GetIntExtra("idVisita", 0);
            }
            else if (activity == "LecturasActivity")
            {
                idLectura = Intent.GetIntExtra("idLectura", 0);
            }
            else { }

            nombreFinca = Intent.GetStringExtra("nombreFinca");
            nombreVisita = Intent.GetStringExtra("visita");
            nombreFecha = Intent.GetStringExtra("fecha");
            

            SetContentView(Resource.Layout.maps);

            RequestPermissions(permissionGroup, 0);

            SupportMapFragment mapfragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapfragment.GetMapAsync(this);

            fincaVisitaText = (TextView)FindViewById(Resource.Id.fincaVisitaText);
            fechaText = (TextView)FindViewById(Resource.Id.fechaText);
            locationButton = (ImageButton)FindViewById(Resource.Id.locationButton);
            locationButton.Click += LocationButton_Click;
            startButton = (ImageButton)FindViewById(Resource.Id.startButton);
            startButton.Click += StartButton_Click;
            changeMap = (Button)FindViewById(Resource.Id.changeMap);
            changeMap.Click += ChangeMap_Click;
            volverMaps = (ImageView)FindViewById(Resource.Id.volverMaps);
            volverMaps.Click += VolverMaps_Click;

            fincaVisitaText.Text = String.Concat(nombreFinca, " ", nombreVisita);
            fechaText.Text = DateTime.Parse(nombreFecha, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

            //Brujula
            // Register for reading changes, be sure to unsubscribe when finished
            Compass.ReadingChanged += Compass_ReadingChanged;

            CreateLocationRequest();            
        }

        private void ChangeMap_Click(object sender, EventArgs e)
        {
            satelite = satelite == false ? true : false;
            map.MapType = satelite == false ? 4 : 1;
            changeMap.Text = satelite == false ? "Mapa" : "Satélite";
        }

        //Brujula
        void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            degrees = (float)data.HeadingMagneticNorth;
            if (currentPositionMarker != null)
            {
                currentPositionMarker.Rotation = degrees - 45;
            }
        }

        public void ToggleCompass()
        {
            try
            {
                if (Compass.IsMonitoring)
                    Compass.Stop();
                else
                    Compass.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Some other exception has occurred
            }
        }

        async void GetDirection()
        {
            //Brujula
            ToggleCompass();

            if (activity == "RecorridoActivity")
            {
                coordenadas = new ObservableCollection<ArrayList>();
                await DB.BringLocation(idVisita, coordenadas);

                ArrayList routeList = new ArrayList();
                routeList = coordenadas[0];

                destinationPoint = (LatLng)routeList.ToArray()[routeList.ToArray().Length - 1];
                startPosition = (LatLng)routeList.ToArray()[0];

                // Location Marker
                MarkerOptions locationMarkerOption = new MarkerOptions();
                locationMarkerOption.SetPosition(startPosition);
                locationMarkerOption.SetTitle("Mi Posición");
                locationMarkerOption.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                Marker locationmarker = map.AddMarker(locationMarkerOption);

                // Destino Marker
                MarkerOptions destinationMarkerOption = new MarkerOptions();
                destinationMarkerOption.SetPosition(destinationPoint);
                destinationMarkerOption.SetTitle("Destino");
                destinationMarkerOption.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                Marker destinationMarker = map.AddMarker(destinationMarkerOption);

                Android.Gms.Maps.Model.Polyline mPolyLine;

                PolylineOptions polylineOptions = new PolylineOptions()
                    .AddAll(routeList)
                    .InvokeWidth(15)
                    .InvokeColor(Color.Goldenrod)
                    .Geodesic(true)
                    .InvokeJointType(JointType.Round);

                mPolyLine = map.AddPolyline(polylineOptions);
            }
            else if (activity == "LecturasActivity")
            {
                puntosLecturas = new ObservableCollection<LatLng>();
                tipo = new ObservableCollection<string>();
                await DB.BringLocationPlanta(idLectura, puntosLecturas, tipo);

                startPosition = puntosLecturas[0];
                MarkerOptions plantaMarkerOption;

                for(int i = 0; i < puntosLecturas.Count; i++)
                {
                    // Planta Marker
                    plantaMarkerOption = new MarkerOptions();
                    plantaMarkerOption.SetPosition(puntosLecturas[i]);
                    plantaMarkerOption.SetTitle(tipo[i]);
                    plantaMarkerOption.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                    Marker plantamarker = map.AddMarker(plantaMarkerOption);
                }
            }
            else { }

            myLastLocation = await locationProviderClient.GetLastLocationAsync();
            myPosition = new LatLng(myLastLocation.Latitude, myLastLocation.Longitude);

            // Current Location Marker
            MarkerOptions positionMakerOption = new MarkerOptions();
            positionMakerOption.SetPosition(myPosition);
            positionMakerOption.SetTitle("Posición Actual");
            positionMakerOption.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.brujula));
            positionMakerOption.SetRotation(degrees - 45);
            positionMakerOption.Anchor((float)0.5, (float)0.5);
            currentPositionMarker = map.AddMarker(positionMakerOption);
        }

        void CreateLocationRequest()
        {
            myLocationRequest = new LocationRequest();
            myLocationRequest.SetInterval(5);
            myLocationRequest.SetFastestInterval(5);
            myLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            myLocationRequest.SetSmallestDisplacement(1);
            myLocationCallback.OnLocationFound += MyLocationCallback_OnLocationFound;
            if (locationProviderClient == null)
            {
                locationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            }
        }

        private void MyLocationCallback_OnLocationFound(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            myLastLocation = e.Location;
            string key = Resources.GetString(Resource.String.mapkey);
            myPosition = new LatLng(myLastLocation.Latitude, myLastLocation.Longitude);
            UpdateLocationToDestination(myPosition, destinationPoint, map, key);

        }

        public void UpdateLocationToDestination(LatLng currentPosition, LatLng destination, GoogleMap map, string key)
        {
            if (currentPositionMarker != null)
            {
                currentPositionMarker.Visible = true;
                currentPositionMarker.Position = currentPosition;
            }
        }

        void StartLocationUpdate()
        {
            locationProviderClient.RequestLocationUpdates(myLocationRequest, myLocationCallback, null);
        }

        private void LocationButton_Click(object sender, EventArgs e)
        {
            DisplayLocation();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            DisplayLocationStart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Length < 1)
            {
                return;
            }

            if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
            {
                DisplayLocationStart();
                StartLocationUpdate();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.ZoomControlsEnabled = true;
            map.MapType = 4;

            if (ChekPermission())
            {
                StartLocationUpdate();
                GetDirection();
            }
        }

        bool ChekPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
            }
            else
            {
                permissionGranted = true;
            }

            return permissionGranted;
        }

        async void DisplayLocationStart()
        {
            if (locationProviderClient == null)
            {
                locationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            }

            myLastLocation = await locationProviderClient.GetLastLocationAsync();
            if (myLastLocation != null)
            {
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(startPosition, 20));
            }
        }

        async void DisplayLocation()
        {
            if (locationProviderClient == null)
            {
                locationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            }

            myLastLocation = await locationProviderClient.GetLastLocationAsync();
            if (myLastLocation != null)
            {
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 20));
            }
        }

        public override void OnBackPressed()
        {
            ExitMaps();
        }

        private void VolverMaps_Click(object sender, EventArgs e)
        {
            ExitMaps();
        }

        private void ExitMaps()
        {
            locationProviderClient.RemoveLocationUpdates(myLocationCallback);
            Finish();
        }
    }
}