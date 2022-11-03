namespace AgapeaDAM.Models
{
    public class Pedido
    {
        #region ....propiedades clase pedido...
        //lista de items pedido, subtotal, gastos de envio, total

        public List<ItemPedido> ItemsPedido { get; set; }
        public Decimal SubTotal { get; set; } = 0;
        public Decimal GastosEnvio { get; set; } = 0;
        public Decimal Total { get; set; } = 0;

        #endregion


        public Pedido()
        {
            this.ItemsPedido = new List<ItemPedido>();
        }

        #region ...metodos clase pedido...

        #endregion
    }
}
