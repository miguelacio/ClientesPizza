
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PizzaClient
{
    [Activity(Label = "OrderDetailActivity")]
    public class OrderDetailActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.order_detail_activity);


            // Create your application here
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
