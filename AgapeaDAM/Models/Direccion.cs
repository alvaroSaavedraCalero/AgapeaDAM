namespace AgapeaDAM.Models
{
    public class Direccion
    {
        #region ....propiedades clase Direccion....
        //calle,numero, piso, letra, edificio, cp , localidad, provincia, pais
        public String IdDireccion { get; set; } = Guid.NewGuid().ToString();
        public String Calle { get; set; }
        public int CP { get; set; }
        public Provincia ProvDirec { get; set; }
        public Municipio MuniDirecc { get; set; }
        public String Pais { get; set; } = "España";
        public Boolean EsPrincipal { get; set; } = false;
        public Boolean EsFacturacion { get; set; } = false;



        #endregion



        #region ...metodos clase Direccion....

        #endregion
    }
}
