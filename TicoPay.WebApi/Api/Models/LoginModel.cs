using System.ComponentModel.DataAnnotations;

namespace TicoPay.Api.Models
{
    /// <summary>
    /// Clase utilizada para contener las credenciales de inicio de Sesión / Class that contains the user credentials
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Obtiene o Almacena el nombre del Tenant o Sub Dominio / Gets or Sets the Tenant name.
        /// </summary>
        /// <value>
        /// Nombre del Tenant o Sub Dominio / Tenant name.
        /// </value>
        [Required]
        public string TenancyName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre de usuario a autentificar / Gets or Sets the User Name.
        /// </summary>
        /// <value>
        /// Nombre de usuario / User name.
        /// </value>
        [Required]
        public string UsernameOrEmailAddress { get; set; }

        /// <summary>
        /// Obtiene o Almacena la contraseña del usuario a autentificar / Gets or Sets the User Password.
        /// </summary>
        /// <value>
        /// Contraseña del usuario / User Password.
        /// </value>
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Clase que contiene el token de sesión / Class that contains the session token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Obtiene el Token de sesión en Ticopays / Gets the session token.
        /// </summary>
        /// <value>
        /// Token de sesión / Session Token.
        /// </value>
        public string TokenAuthenticate { get; set; }

        
    }

    /// <summary>
    /// Clase que contiene las opciones a utilizar para extender el tiempo de vida del token de sesión / Class that contains the selected options to extend the session token time
    /// </summary>
    public class RefreshCredentials
    {
        /// <summary>
        /// Obtiene o Almacena el Token de sesión original obtenido con el método Authenticate / Gets or Sets the original session token obtained during authentication.
        /// </summary>
        /// <value>
        /// Token de sesión original / Original session token.
        /// </value>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Almacena el tipo de extensión de tiempo a solicitar : {Minutos , Horas , Días}
        /// Sets the time extension to be requested : {Minutes , Hours , Days}
        /// </summary>
        /// <value>
        /// Tipo de extension de tiempo : {Minutos , Horas , Días} / Time Extension type : {Minutes , Hours , Days}.
        /// </value>
        [Required]
        public TimeLapsType AdditionalTimeType { get; set; }

        /// <summary>
        /// Almacena la cantidad de tiempo de extensión a solicitar.
        /// Minutos {entre 1.0 y 120.0} , Horas {entre 0.1 y 24.0} , Días {entre 1.0 y 30.0}
        /// Sets the amount of time of the extension.
        /// Minutes {between 1.0 and 120.0} , Hours {between 0.1 and 24.0} , Days {between 1.0 and 30.0}
        /// </summary>
        /// <value>
        /// Cantidad de tiempo / Amount of time.
        /// </value>
        [Required]
        public double AdditionalTimeAmount { get; set; }
    }

    /// <summary>
    /// Enumerado que especifica los Tipos de extensiones de tiempo disponibles para extender la duración del token / Enum that specify the time extension types
    /// </summary>
    public enum TimeLapsType
    {
        /// <summary>
        /// Minutos / Minutes
        /// </summary>
        Minutes = 0,
        /// <summary>
        /// Horas / Hours
        /// </summary>
        Hours = 1,
        /// <summary>
        /// Días / Days
        /// </summary>
        Days = 2,
    }


}