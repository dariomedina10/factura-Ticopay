using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicoPayDll;
using TicoPayDll.Authentication;
using TicoPayDll.Response;
using System.Configuration;
using System.Threading;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FirmadorTicopay
{
    public partial class Login : Form
    {
        private string _token = "";
        private string _tenant = "";

        public Login()
        {   
            InitializeComponent();
            _tenant = ConfigurationManager.AppSettings["tenant"];
            if ((_tenant == null) || (_tenant.Length == 0))
            {
                MessageBox.Show("El Sub Dominio no puede estar vacio, Contacte a soporte para solucionarlo");
            }
            else
            {
                this.Text = "Firmador Ticopay " + _tenant;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if ((tbUsuario.Text.Length == 0) || (tbClave.Text.Length == 0))
            {
                MessageBox.Show("Debe introducir el nombre de Usuario y la Clave");
                return;
            }
            this._token = AutentificarUsuario(_tenant, tbUsuario.Text, tbClave.Text);
            if ((this._token != "") && (this._token != null))
            {                
                var thread = new Thread(
                    () => IniciarApp(_token));
                thread.Start();
                this.Close();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Usuario o Clave incorrectos");
                return;
            }
        }

        public static void IniciarApp(string token)

        {
            Application.Run(new MenuPrincipal(token));

        }

        public string AutentificarUsuario(string tenancy, string user, string password)
        {

            TicoPayDll.Response.Response respuestaServicio;
            TicoPayDll.Authentication.UserCredentials credenciales = new TicoPayDll.Authentication.UserCredentials();
            credenciales.tenancyName = tenancy;
            credenciales.usernameOrEmailAddress = user;
            credenciales.password = password;
            respuestaServicio = InternalAuthenticate(credenciales).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonAuthentication token = JsonConvert.DeserializeObject<JsonAuthentication>(respuestaServicio.result);
                return token.objectResponse.tokenAuthenticate;
            }
            else
            {                
                return null;
            }
        }

        #region InternalAuthenticate

        /// <summary>
        /// Authenticates the specified user in the Web Api.
        /// </summary>
        /// <param name="bodyInformation">Contains the Tenant, user and password information.</param>
        /// <returns>Ok if login, Bad Request if incorrect Tenant , user or password information</returns>
        static async public Task<TicoPayDll.Response.Response> InternalAuthenticate(UserCredentials bodyInformation)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                return await InternalAuthenticateAsync(httpClient, bodyInformation);
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                TicoPayDll.Response.Response methodResponse = new TicoPayDll.Response.Response();
                methodResponse.status = ResponseType.BadRequest;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        /*
         *  Metodo para realizar el Request de la Autentificacion y procesar la respuesta 
        */
        static async Task<TicoPayDll.Response.Response> InternalAuthenticateAsync(HttpClient httpClient, UserCredentials loginInformation)
        {
            string path = Config.webServiceUrl + "Account/InternalAuthenticate";
            TicoPayDll.Response.Response methodResponse = new TicoPayDll.Response.Response();
            string json = JsonConvert.SerializeObject(loginInformation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }
}
