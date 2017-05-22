using System;
using System.Collections.Generic;

namespace PizzaClient
{
	public class Pizza
		{
		public string nombre { get; set; }
			public int id { get; set; }
			public string ingredientes { get; set; }
			public string calificacion { get; set; }
			public int tiempo_de_preparacion { get; set; }
			public double precio { get; set; }
		}
}
