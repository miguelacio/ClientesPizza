using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace PizzaClient
{
	public class PedidoAdapter : RecyclerView.Adapter
	{

		List<Pedido> pedidoList = new List<Pedido>();
		public event EventHandler<int> ItemClick;

		public PedidoAdapter(List<Pedido> pedidoList)
		{
			this.pedidoList = pedidoList;

		}
		public override int ItemCount
		{
			get
			{
		return pedidoList.Count;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			ViewHolder cv = holder as ViewHolder;

            String id = pedidoList[position].id_pizza.ToString();
            cv.textViewNombre.Text = pedidoList[position].nombre_cliente;

            cv.textViewPizza.Text = pedidoList[position].pizza;
			cv.textViewId.Text = "Id de pedido: " + id;
            cv.textViewDir.Text = pedidoList[position].direccion;
            cv.textViewEstado.Text = pedidoList[position].pizza;
		}


		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout_pedido, parent, false);
			ViewHolder cv = new ViewHolder(itemView, OnClick);
			return cv;
		}

		public class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView textViewNombre { get; private set; }
			public TextView textViewPizza { get; private set; }
			public TextView textViewId { get; private set; }
            public TextView textViewDir { get; private set; }
            public TextView textViewEstado { get; private set; }
			public ViewHolder(View itemView, Action<int> listener) : base(itemView)
			{
                
				textViewNombre = itemView.FindViewById<TextView>(Resource.Id.textView1);
				textViewPizza = itemView.FindViewById<TextView>(Resource.Id.textView2);
				textViewId = itemView.FindViewById<TextView>(Resource.Id.textView3);
                textViewDir = itemView.FindViewById<TextView>(Resource.Id.textView5);
                textViewEstado = itemView.FindViewById<TextView>(Resource.Id.textView4);
				itemView.Click += (sender, e) => listener(base.Position);
			}		
		}

		void OnClick(int position)
		{
			if (ItemClick != null)
			ItemClick(this, position);
		}
	}
}
