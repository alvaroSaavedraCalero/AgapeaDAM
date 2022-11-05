using AgapeaDAM.Models.Interfaces;

// instalando paquete NUGET mailjet.api
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace AgapeaDAM.Models
{
    public class EmailClienteMailJet : IClienteEmail
    {

        #region propiedades de clase
        // Usamos los datos de la propia api de MailJet
        public String userId { get; set; } = "c191d3098b901c4925ed2e8d4ef798b7"; // clave api conf.servidor smtp
        public String key { get; set; } = "75a1cda61615c7d86bcadf7e2c245661"; // clave secreta conf.servidor smtl mailjet

        #endregion

        #region metodos de la clase

        // el async y el Task<Boolean> es para que el await pueda funcionar, ya que tiene que esperar al resultado de la operacion
        public async Task<Boolean> enviarEmail(String emailCliente, String subject, String body, String? ficheroAdjunto)
        {
            // Creamos un objeto de tipo MailjetClient
            MailjetClient mailjetClient = new MailjetClient(this.userId, this.key);

            // Creamos un email para mandarlo usando ese cliente MailjetClient
            // segun la api del paquete Mailjet.Api es un objeto de la clase MailjetRequest

            MailjetRequest request = new MailjetRequest() { Resource = Send.Resource }
                .Property(Send.FromEmail, "pmr.aiki@gmail.com") // el email desde donde se van a mandar
                .Property(Send.FromName, "adminApagea")
                .Property(Send.Subject, subject)
                .Property(Send.TextPart, "")
                .Property(Send.HtmlPart, body)
                .Property(Send.Recipients, new JArray
                {
                    new JObject
                    {
                        {"Email", emailCliente},
                        {"Name", "Passenger 1" }
                    }
                });

            // mando objeto MailjetRequest usando metodo .PostAsyns() del objeto MailJetClient
            // el resultado de la operacion es un objeto clase MailJetResponse
            // await lo que hace es esperar a que el 
            MailjetResponse respuestaEnvioEmail = await mailjetClient.PostAsync(request);

            return respuestaEnvioEmail.IsSuccessStatusCode;
        }

        #endregion
    }
}
