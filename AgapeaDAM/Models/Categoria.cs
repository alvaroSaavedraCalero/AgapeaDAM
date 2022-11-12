namespace AgapeaDAM.Models
{
    public class Categoria
    {
        #region ... propiedades de clase ...
        public String IdCategoria { get; set; } = "";
        public String NombreCategoria { get; set; } = "";
        #endregion

        #region ... metodos de la clase ...

        public Categoria(String IdCategoia, String NombreCategoria)
        {
            this.IdCategoria = IdCategoia;
            this.NombreCategoria = NombreCategoria;
        }

        #endregion
    }
}
