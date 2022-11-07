using System.ComponentModel.DataAnnotations; //<---- namespace donde estan el conjunto de clases Validators

namespace AgapeaDAM.Models
{
    public class Cuenta
    {
        #region ....propiedades de la clase cuenta....

        public String IdCuenta { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage ="*Login obligatorio")]
        [MaxLength(50,ErrorMessage ="*Maximo numero de caracteres permitidos es de 50")]
        public String? Login { get; set; } //validadores required y maxlength

        [Required(ErrorMessage ="*Email obligatorio")]
        [EmailAddress(ErrorMessage ="*Formato de email incorrecto")]
        public String? Email { get; set; } //validadores required y patron formato email

        [Required(ErrorMessage ="*Contraseña obligatoria")]
        [MinLength(6,ErrorMessage ="*El tamaño minimo de la contraseña debe ser de 6 caracteres")]
        [MaxLength(50, ErrorMessage ="*El tamaño maximo de la contraseña es de 50 caracteres")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_]).{6,50}$",ErrorMessage ="*La contraseña debe tener al menos una MAYS, mins, digito y caracter alfanumerico")]
        public String?  Password { get; set; } // validadores required, minlength, maxlength y formato password para fortaleza

        public Boolean CuentaActivada { get; set; } = false;

        // nombre del fichero imagen dentro del directorio www-root/imagen/avataresClientes
        public String? ImagenAvatar { get; set; } = "";

        // contenido en base64 de ese fichero imagen
        public String? ImagenAvatarBASE64 { get; set; } = "";
        #endregion



        #region ....metodos de la clase cuenta....

        #endregion
    }
}
