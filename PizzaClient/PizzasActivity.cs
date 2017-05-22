
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace PizzaClient
{
	[Activity(Label = "PizzasActivity")]
	public class PizzasActivity : Activity
	{

		RecyclerView mRecyclerView;
		RecyclerView.LayoutManager mLayoutManager;
        PizzaAdapter pizzaAdapter;
        List<Pizza> pizzasList;

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
			button.Click += delegate
			{
				builder.Dismiss();
				Toast.MakeText(this, "Alert dialog dismissed!", ToastLength.Short).Show();
			};
			builder.Show();

		}
	}
}
