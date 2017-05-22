
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace PizzaClient
{
	[Activity(Label = "PizzasActivity")]
	public class PizzasActivity : Activity, ILocationListener
	{

		RecyclerView mRecyclerView;
		RecyclerView.LayoutManager mLayoutManager;
        PizzaAdapter pizzaAdapter;
        List<Pizza> pizzasList;

		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_pizzas);

			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			mLayoutManager = new LinearLayoutManager(this);
			mRecyclerView.SetLayoutManager(mLayoutManager);

			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Obteniendo pizzas", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
					pizzasList = await getPizzas();
					pizzaAdapter = new PizzaAdapter(pizzasList);
					pizzaAdapter.ItemClick += OnItemClick;
					mRecyclerView.SetAdapter(pizzaAdapter);
					progressDialog.Dismiss();
				}
				);
			})).Start();

            InitializeLocationManager();
		}


		public async Task<List<Pizza>> getPizzas()
		{
			string baseurl = "https://segundoproyecto.azurewebsites.net/api/pizzasapi";
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uri = new Uri(baseurl);
			var response = await Client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var items = JsonConvert.DeserializeObject<List<Pizza>>(content);
				Toast.MakeText(this, "Success", ToastLength.Long).Show();
				return items;
			}
			else
			{
				Toast.MakeText(this, "There was an error please try again later.", ToastLength.Long).Show();
				return null;
			}
		}


		void OnItemClick(object sender, int e)
		{
			Pizza pizza = pizzasList.ElementAt(e);


			//Inflate layout
            View view = LayoutInflater.Inflate(Resource.Layout.dialog_item_layout, null);
			AlertDialog builder = new AlertDialog.Builder(this).Create();
			builder.SetView(view);
			builder.SetCanceledOnTouchOutside(false);
			Button button = view.FindViewById<Button>(Resource.Id.btnClearLL);
            Button buttonPedido = view.FindViewById<Button>(Resource.Id.btnLoginLL);
            EditText nombre = view.FindViewById<EditText>(Resource.Id.txtUsername);
            EditText direccion = view.FindViewById<EditText>(Resource.Id.txtUsername);
            TextView textViewTitle = view.FindViewById<TextView>(Resource.Id.titleLogin);

            textViewTitle.Text = "Pedir una pizza " + pizza.nombre;
			buttonPedido.Click += delegate
			{
				_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);

				Pedido p = new Pedido();
                p.pizza = pizza.nombre;
                p.nombre_cliente = nombre.Text.ToString();
                p.direccion = direccion.Text.ToString();
                if (_currentLocation != null)
                {
                    p.latitud = _currentLocation.Latitude.ToString();
                    p.longitud = _currentLocation.Longitude.ToString();

                }



				var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Creando pedido", true);
				new System.Threading.Thread(new ThreadStart(delegate
				{
					RunOnUiThread(async () =>
					{
						
						string url = "https://segundoproyecto.azurewebsites.net/api/pedidosapi";
						JsonValue json = await makeJSONRequest(url, p);

						progressDialog.Dismiss();
					}
					);
				})).Start();


			};

            button.Click += delegate
		   {
			   builder.Dismiss();

		   };
			builder.Show();

		}

		private async Task<JsonValue> makeJSONRequest(string url, Pedido p)
		{

			String post = JsonConvert.SerializeObject(p);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json";
			request.Method = "POST";

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
				
			}
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
