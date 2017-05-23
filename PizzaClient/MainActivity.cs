using Android.App;
using Android.Widget;
using Android.OS;

namespace PizzaClient
{
	[Activity(Label = "PizzaClient", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		ImageView imageViewPedidos;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			//imageViewPedir = FindViewById<ImageView>(Resource.Id.image_view_pedir);
			imageViewPedidos = FindViewById<ImageView>(Resource.Id.image_view_pedidos);

			//imageViewPedir.Click += delegate { StartActivity(typeof(PizzasActivity)); };
			imageViewPedidos.Click += delegate { StartActivity(typeof(OrdersActivity)); };
		}
	}
}

