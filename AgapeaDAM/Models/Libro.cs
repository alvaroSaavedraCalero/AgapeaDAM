namespace AgapeaDAM.Models
{
    public class Libro
    {
        #region ...propiedades de la clase libro...
        //imagen libro, titulo, editorial, autores, edicion, numero paginas, dimensiones, idioma, isbn-10,isbn-13,
        //encuadernacion, precio, resumen
        public String ImagenLibro { get; set; }
        public String Titulo { get; set; }
        public String Editorial { get; set; }
        public String Autores { get; set; }
        public String  Edicion { get; set; }

        public int NumeroPaginas { get; set; }
        public String Dimensiones { get; set; }
        public String Idioma { get; set; } = "Español";

        public String ISBN10 { get; set; }
        public String ISBN13 { get; set; }
        public String Encuadernacion { get; set; } = "Tapa Blanda. Con solapas";
        public Decimal Precio { get; set; } = 0;
        public String Resumen { get; set; }

        #endregion


        #region ....metodos de la clase libro.....

        #endregion
    }
}
