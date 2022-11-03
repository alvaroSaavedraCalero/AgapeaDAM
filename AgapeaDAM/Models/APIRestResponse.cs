using System.Text.Json;

namespace AgapeaDAM.Models
{
    // Clase para mapear respuesta de servicio REST para obtener provs y municipios
    
    public class APIRestResponse<T>
    {
        // json con las siguientes propiedades {update_date: '', dize: xx, data [array de objetos], warning; ''}
        #region propiedades de la clase

        public String Update_date { get; set; }
        public int Size { get; set; }
        public List<JsonElement> Data { get; set; }
        public String Warning { get; set; }

        #endregion


        #region metodos de la clase

        public List<T> datosConvertidos(List<JsonElement> data)
        {
            List<T> datosConvertidos = new List<T>();
            foreach (JsonElement item in data)
            {
                datosConvertidos.Add(JsonSerializer.Deserialize<T>(item));
            }
            return datosConvertidos;
        }

        #endregion
    }
}
