namespace AgapeaDAM.Models
{
    public class Pedido
    {
        #region ....propiedades clase pedido...
        //lista de items pedido, subtotal, gastos de envio, total

        public String IdPedido { get; set; } = Guid.NewGuid().ToString();
        public List<ItemPedido> ItemsPedido { get; set; }
        public Decimal SubTotal { get => this.calculaSubTotal(); }
        public Decimal GastosEnvio { get; set; } = 0;
        public Decimal Total { get => this.SubTotal + this.GastosEnvio }
        public String IdDireccionEnvio { get; set; } = "";
        public String IdDireccionFacturacion { get; set; } = "";
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        #endregion


        public Pedido()
        {
            this.ItemsPedido = new List<ItemPedido>();
        }

        #region ...metodos clase pedido...

        private Decimal calculaSubTotal()
        {
            Decimal subtotal = 0;
            this.ItemsPedido.ForEach(
                (ItemPedido item) => subtotal += item.LibroItem.Precio * item.CantidadItem
                );
            return subtotal;
        }

        #endregion
    }
}
