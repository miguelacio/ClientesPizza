using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace PizzaClient
{
	public class PizzaAdapter : RecyclerView.Adapter
	{
		List<Pizza> pizzaList;
		public event EventHandler<int> ItemClick;
		
		public PizzaAdapter(List<Pizza> pizzaList)
		{
			this.pizzaList = pizzaList;
		}

		public override int ItemCount
		{
			get
			{
				return pizzaList.Count;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{

			ViewHolder viewHolder = holder as ViewHolder;
	
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			throw new NotImplementedException();
		}

		 public class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView Nombre { get; private set; }
			public TextView ingredients { get; private set; }
				public ViewHolder(View itemView, Action<int> listener) : base(itemView)
				{
				ingredients = itemView.FindViewById<TextView>(Resource.Id.textView2);
				Nombre = itemView.FindViewById<TextView>(Resource.Id.textView1);
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
