using System;
namespace PizzaClient
{
    public class Pedido
    {
        
        public string nombre_cliente { get; set; }
        public string pizza { get; set; }
        public int id_pizza { get; set; }
        public string direccion { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
    }
}
