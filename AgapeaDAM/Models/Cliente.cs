using System.ComponentModel.DataAnnotations; //<---- namespace donde estan el conjunto de clases Validators

namespace AgapeaDAM.Models
{
    public class Cliente
    {
        #region ....propiedades de clase....
        /*
            propiedades de la clse cliente Nombre, Apellidos, Email, Login, Password, Telefono
            lista de direcciones, pedidos, deseos, imagen avatar
        
         */

        public String IdCliente { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage ="*Nombre es obligatorio")] //1º validador sobre propiedad Nombre
        [MaxLength(100,ErrorMessage="Maximo tamaño permitido 100 caracteres")] //2º validador sobre propiedad Nombre
        public String? Nombre { get; set; }


        [Required(ErrorMessage = "*Apellidos son obligatorios")] //1º validador sobre propiedad Apellidos
        [MaxLength(500, ErrorMessage = "Maximo tamaño permitido 500 caracteres")] //2º validador sobre propiedad Apellidos
        public String? Apellidos { get; set;}

        [Required(ErrorMessage = "*Telefono es obligatorio")] //1º validador sobre propiedad Telefono
        [RegularExpression(@"^\d{3}(\s\d{2}){3}$",ErrorMessage ="*Formato de Tlfno invalido, ej: 666 11 22 33")] //2º validador sobre propiedad Telefono
        public String?  Telefono { get; set; }


        public Cuenta? CuentaCliente { get; set; }
        public String? NIF { get; set; } = "";

        public String Genero { get; set; } = "";
        public String Descripcion { get; set; } = "";

        public DateTime? FechaNacimiento { get; set; } = DateTime.Now;
        public List<Direccion> MisDireciones { get; set; }
        //public Dictionary<String,Direccion> MisDirecciones { get; set; }

        public List<Pedido> MisPedidos { get; set; }
        public List<Lista> MisListas { get; set; }

        public Pedido PedidoActual { get; set; } = new Pedido();

        #endregion


        public Cliente()
        {
            this.CuentaCliente = new Cuenta();
            this.MisDireciones = new List<Direccion>();
            this.MisPedidos = new List<Pedido>();
            this.MisListas = new List<Lista>();
        }


        #region ....metodos de clase....

        #endregion
    }
}
