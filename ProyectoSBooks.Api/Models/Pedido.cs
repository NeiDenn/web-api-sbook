﻿namespace ProyectoSBooks.Api.Models
{
    public class Pedido
    {
        public int idpedido { get; set; }
        public DateTime fpedido { get; set; }
        public string dni { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
    }
}
