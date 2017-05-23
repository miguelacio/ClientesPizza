
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
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace PizzaClient
{
    [Activity(Label = "OrderDetailActivity")]
    public class OrderDetailActivity : Activity
    {
        Pedido currentPedido;
        String id;
        TextView textViewPizza, textViewStatus, textViewClient, textViewAddress;
        Button buttonPickUp, buttonDeliver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.order_detail_activity);
            textViewPizza = FindViewById<TextView>(Resource.Id.text_view_pizza);
            textViewStatus = FindViewById<TextView>(Resource.Id.text_view_status);
            textViewClient = FindViewById<TextView>(Resource.Id.text_view_client);
            textViewAddress = FindViewById<TextView>(Resource.Id.text_view_address);

            buttonPickUp = FindViewById<Button>(Resource.Id.button_pick_up);
            buttonDeliver = FindViewById<Button>(Resource.Id.buttonPanel);


			int idint = Intent.GetIntExtra("id", 0);
            id = idint.ToString();

			var progressDialog = ProgressDialog.Show(this, "Espere un momento", "Obteniendo detalles de pedido", true);
			new System.Threading.Thread(new ThreadStart(delegate
			{
				RunOnUiThread(async () =>
				{
                    currentPedido = await getPedidoInfo(id);
                    textViewPizza.Text = currentPedido.pizza;
                    textViewClient.Text = currentPedido.nombre_cliente;
                    textViewAddress.Text = currentPedido.direccion;
                    textViewStatus.Text = currentPedido.id_pizza.ToString();
					progressDialog.Dismiss();
				}
				);
			})).Start();
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
				//Console.WriteLine("This fucking api is working");
				Toast.MakeText(this, "Success", ToastLength.Long).Show();
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
            int bandera = 0;

            if (bandera == 1)
			{
				base.OnBackPressed();
            } else{
                Toast.MakeText(this, "Alto ahí", ToastLength.Long).Show();
            }
		}


    }
}
