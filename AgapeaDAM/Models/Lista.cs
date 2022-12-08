namespace AgapeaDAM.Models
{
    public class Lista
    {
        public String IdLista { get; set; } = Guid.NewGuid().ToString();
        public String IdCliente { get; set; }
        public String Titulo { get; set; }
        public List<Libro> ListaLibros { get; set; }
        public String Descripcion { get; set; }
        public String ImagenListaBASE64 { get; set; } = "C:\\Users\\saave\\source\\repos\\AgapeaDAM (1)\\AgapeaDAM\\AgapeaDAM\\wwwroot\\";


        public Lista()
        {
            this.ListaLibros = new List<Libro>();

        }
    }
}
