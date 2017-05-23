using System;
namespace PizzaClient
{
    public class Pedido
    {
        
        public string nombre_cliente { get; set; }
        public string pizza { get; set; }
        public int id { get; set; }
        public int id_pizza { get; set; }
        public string direccion { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
		public string estado { get; set; }
        public double Repartidor_latitud { get; set; }
        public double Repartidor_longitud { get; set; }
    }
}
