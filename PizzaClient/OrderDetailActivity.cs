
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace PizzaClient
{
    [Activity(Label = "OrderDetailActivity")]
    public class OrderDetailActivity : Activity, ILocationListener
    {
        Pedido currentPedido;
        String id;
        TextView textViewPizza, textViewStatus, textViewClient, textViewAddress;
        Button buttonPickUp, buttonDeliver;

        //GPS
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;


		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.order_detail_activity);
            textViewPizza = FindViewById<TextView>(Resource.Id.text_view_pizza);
            textViewStatus = FindViewById<TextView>(Resource.Id.text_view_status);
            textViewClient = FindViewById<TextView>(Resource.Id.text_view_client);
            textViewAddress = FindViewById<TextView>(Resource.Id.text_view_address);

            buttonPickUp = FindViewById<Button>(Resource.Id.button_pick_up);
            buttonDeliver = FindViewById<Button>(Resource.Id.button_deliver);


			int idint = Intent.GetIntExtra("id", 0);
            id = idint.ToString();

            MakeMainRequest(id);

			

            buttonPickUp.Click += delegate { PickUpOrder(); };

            buttonDeliver.Click += delegate { DeliverOrder(); };

            InitializeLocationManager();
        }

        private void DeliverOrder()
        {
			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Entregando  orden", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
					string url = "http://segundoproyecto.azurewebsites.net/api/pedidosapi/" + id; ;
					Pedido p = currentPedido;
					p.estado = "Entregado";
					if (_currentLocation != null)
					{
						p.Repartidor_latitud = _currentLocation.Latitude;
						p.Repartidor_longitud = _currentLocation.Longitude;
					}
					JsonValue json = await makeJSONRequest(url, p);



					progressDialog.Dismiss();

				}
				);
			})).Start();

			MakeMainRequest(id);
        }

        private void MakeMainRequest(string id)
        {
			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Obteniendo detalles de pedido", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
					currentPedido = await getPedidoInfo(id);

					fillstuff(currentPedido);
					progressDialog.Dismiss();
				}
				);
			})).Start();
        }

        private void PickUpOrder()
        {
			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Recogiendo  orden", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
					string url = "http://segundoproyecto.azurewebsites.net/api/pedidosapi/" + id; ;
					Pedido p = currentPedido;
					p.estado = "En Camino";
                    if (_currentLocation != null)
                    {
                        p.Repartidor_latitud = _currentLocation.Latitude;
                        p.Repartidor_longitud = _currentLocation.Longitude;
                    }
					JsonValue json = await makeJSONRequest(url, p);



					progressDialog.Dismiss();

				}
				);
			})).Start();

            MakeMainRequest(id);
        }

        private void fillstuff(Pedido currentPedido)
        {
			textViewPizza.Text = currentPedido.pizza;
			textViewClient.Text = currentPedido.nombre_cliente;
			textViewAddress.Text = currentPedido.direccion;
            textViewStatus.Text = currentPedido.estado;


			switch (currentPedido.estado)
			{

				case "Aceptado":
					buttonPickUp.Enabled = false;
					break;
				case "En camino":
                    buttonPickUp.Enabled = false;
                    buttonDeliver.Enabled = false;
					break;
				case "Terminado":
                    buttonDeliver.Enabled = false;		
					break;
				default:
					buttonPickUp.Enabled = false;
					buttonDeliver.Enabled = false;
					break;

			}
        }

		private async Task<JsonValue> makeJSONRequest(string url, Pedido p)
		{
			String post = JsonConvert.SerializeObject(p);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json";
			request.Method = "PUT";

			var stream = await request.GetRequestStreamAsync();
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(post);
				writer.Flush();
				writer.Dispose();
			}

			var response = await request.GetResponseAsync();


			var respStream = response.GetResponseStream();

			using (StreamReader sr = new StreamReader(respStream))
			{
				return sr.ReadToEnd();

			}
		}



		public async Task<Pedido> getPedidoInfo(string id)
		{
			string baseurl = "https://segundoproyecto.azurewebsites.net/api/pedidosapi/" + id;
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uri = new Uri(baseurl);
			var response = await Client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<Pedido>(content);
				Toast.MakeText(this, "Petición exitosa", ToastLength.Long).Show();
				return item;

			}
			else
			{
				Toast.MakeText(this, "There was an error please try again later.", ToastLength.Long).Show();
				return null;
			}
		}

		public override void OnBackPressed()
		{
            if (currentPedido.estado == "En Camino")
            {
                Toast.MakeText(this, "No puedes salirte mientras tengas un pedido en camino.", ToastLength.Long).Show();
            }
            else
            {
                base.OnBackPressed();
            }
		}
        //GPS

		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			if (_currentLocation == null)
			{
				Toast.MakeText(this, "Not success", ToastLength.Long).Show();
			}
			else
			{

				Toast.MakeText(this, string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude), ToastLength.Long).Show();
				if (currentPedido.estado == "En camino")
				{
                    Pedido p = currentPedido;
                    p.Repartidor_latitud = _currentLocation.Latitude;
                    p.Repartidor_longitud = _currentLocation.Longitude;
                    string url = "http://segundoproyecto.azurewebsites.net/api/pedidosapi/" + id; ;
                    requestHelperAsync(p, url);
				} 

			}
		}

        private async Task requestHelperAsync(Pedido p, string url)
        {
            JsonValue json = await makeJSONRequest(url, p);
        }

        public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
			//Log.Debug("GPS", "{0}, {1}", provider, status);
		}

		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}
			//Log.Debug("GPS", "Using " + _locationProvider + ".");

		}

		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
			//Log.Debug("GPS", "Listening for location updates using " + _locationProvider + ".");

		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
			//Log.Debug("GPS", "No longer listening for location updates.");
		}


	}
}
