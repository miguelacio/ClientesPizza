using System;
using System.Collections.Generic;

namespace PizzaClient
{
	public class Pizza
		{
		public string nombre { get; set; }
			public int Id { get; set; }
			public List<String> Ingredientes { get; set; }
			public int Calificacon { get; set; }
			public int TiempoPreparacion { get; set; }
			public double Precio { get; set; }
		}
}
