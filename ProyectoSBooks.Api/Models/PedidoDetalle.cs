﻿namespace ProyectoSBooks.Api.Models
{
    public class PedidoDetalle
    {
        public int idpedido { get; set; }
        public int idproducto { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
    }
}
