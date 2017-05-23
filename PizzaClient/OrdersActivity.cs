
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
	[Activity(Label = "OrdersActivity")]
	public class OrdersActivity : Activity
	{

		RecyclerView mRecyclerView;
		RecyclerView.LayoutManager mLayoutManager;
		List<Pedido> pedidoList;
		PedidoAdapter pedidoAdapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_orders);

			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			mLayoutManager = new LinearLayoutManager(this);
			mRecyclerView.SetLayoutManager(mLayoutManager);

			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Obteniendo pizzas", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
					pedidoList = await getPedidos();
					pedidoAdapter = new PedidoAdapter(pedidoList);
					pedidoAdapter.ItemClick += OnItemClick;
					mRecyclerView.SetAdapter(pedidoAdapter);
					progressDialog.Dismiss();
				}
				);
			})).Start();

		}

		public async Task<List<Pedido>> getPedidos()
		{
			string baseurl = "https://segundoproyecto.azurewebsites.net/api/pedidosapi";
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uri = new Uri(baseurl);
			var response = await Client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var items = JsonConvert.DeserializeObject<List<Pedido>>(content);
					Toast.MakeText(this, "petición exitosa", ToastLength.Long).Show();
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
					Pedido pedido = pedidoList.ElementAt(e);
                    
                


	    		}			

	
	}
}
