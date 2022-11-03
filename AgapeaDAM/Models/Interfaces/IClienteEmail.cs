namespace AgapeaDAM.Models.Interfaces
{
    
    public interface IClienteEmail
    {
        #region propiedades de la interface 
        public String userId { get; set; }
        public String key { get; set; }

        #endregion

        #region metodos de la interface
        public Task<Boolean> enviarEmail(String emailCliente, String subject, String body, String? ficheroAdjunto);

        #endregion
    }
}
