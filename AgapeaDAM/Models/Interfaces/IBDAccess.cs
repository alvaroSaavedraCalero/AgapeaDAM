namespace AgapeaDAM.Models.Interfaces
{
    //interface q define props y metodos q van a tener todas las clases (modelo) SERVICIO q van a meterse en el modulo
    //de inyeccion de dependencias
    public interface IBDAccess
    {
        public String CadenaConexionSever { get; set; }
        public Boolean RegistrarClliente(Cliente clienteAguardar);
        public Cliente ExisteCliente(String email, String password);
        public Boolean ComprobarCuentaActiva(String email);

        public Boolean activarCuentaCliente(String idCuenta);

        public Boolean operarDireccion(Direccion nuevaDir, String idCliente, String operacion);

        public Boolean updateCuentaSubirImagen(String nombreFichero, String contenidoBASE64, String idCuenta);
    }
}
