using System;
using TicoPay.Clients;
using TicoPay.MultiTenancy;
using System.Collections.Generic;
using System.IO;
using System.Net;
using IdentityModel.Client;
using TicoPay.Core.Common;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using TicoPay.Common;
using TicoPay.Invoices.XSD;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using TicoPay.Vouchers;
using TicoPay.Common;

namespace TicoPay.Invoices.Dto
{

    public class ValidateTribunet
    {
        public const string ClientSecret = "";

        //***************************************************************************************************************************
        // * credenciales de desarrollo de ticopay con hacienda
        //// * *************************************************************************************************************************/
        ////public const string Username = "cpj-3-101-741788@stag.comprobanteselectronicos.go.cr";
        ////public const string Password = "@9m]Ga>S7!8%}]Ax;-E]";
        ///***************************************************************************************************************************
        // ***************************************************************************************************************************/

        //***************************************************************************************************************************
        // * credenciales de produccion de ticopay con hacienda
        //// * *************************************************************************************************************************/
        //public const string Username = "cpj-3-101-741788@prod.comprobanteselectronicos.go.cr";
        //public const string Password = ".#/bj1+BV.@$S%Pve#:x";
        ///***************************************************************************************************************************
        // ***************************************************************************************************************************/

        public static string ClientId => ConfigurationManager.AppSettings["ClientId"];
        public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"];
        public static string WebApiNewsEndPoint => ConfigurationManager.AppSettings["WebApiNewsEndPoint"];
        public static string requestUri => ConfigurationManager.AppSettings["requestUri"];

        public static string Scopes = "sast_rest_ap";
        public static readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>()
    {
        { "membershipProvider", "Default" }
    };

        // url de Tribunet
        public const string ApiComprobanteElectronicoUrl = "https://api.comprobanteselectronicos.go.cr/";

        private static TokenClient tokenClient;

        public static double TribunetTimeOut => Double.Parse(ConfigurationManager.AppSettings["TribunetTimeOut"]);

        public string SendResponseTribunet(string method, Client client, Tenant tenant, Invoice invoice, string voucherkey, Note note, Voucher voucher)
        {
            string reponseHtml = "-1";
            try
            {
                var pss =  CryptoHelper.Desencriptar(tenant.PasswordTribunet);

                tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues);
                TokenResponse tokenResponse = RequestToken(tenant.UserTribunet, pss);
                string accessToken = tokenResponse.AccessToken;
                //The purpose of the refresh token is to retrieve new access token when the ols expires
                string refreshToken = tokenResponse.RefreshToken;
                reponseHtml = CallApi(accessToken, method, client, tenant, invoice, voucherkey, note, voucher);
                var newTokenResponse = RefreshToken(refreshToken);
            }
            catch (Exception ex)
            {
                invoice.ResponseTribunetExepcion = "Error: " + ex.Message + "\n" + ex.StackTrace;
            }
            return reponseHtml;
        }

        public string SendResponseTribunet(string method, Client client, Tenant tenant, Invoice invoice, string voucherkey, Note note, Voucher voucher, out TokenResponse token)
        {
            string reponseHtml = "-1";
            string refreshToken = null;
            TokenResponse tokenResponse = null;
            try
            {
                var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);

                tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues);
                tokenClient.Timeout = TimeSpan.FromSeconds(TribunetTimeOut);
                tokenResponse = RequestToken(tenant.UserTribunet, pss);
                string accessToken = tokenResponse.AccessToken;
                //The purpose of the refresh token is to retrieve new access token when the ols expires
                refreshToken = tokenResponse.RefreshToken;
                reponseHtml = CallApi(accessToken, method, client, tenant, invoice, voucherkey, note, voucher);
                
            }
            catch (Exception ex)
            {
                invoice.ResponseTribunetExepcion = "Error: " + ex.Message + "\n" + ex.StackTrace;
            }
            token = (refreshToken != null) ? RefreshToken(refreshToken) : null;
            return reponseHtml;
        }


        public string SendResponseTribunet(string method, Tenant tenant, string voucherkey, DateTime dueDate,string path)
        {
            string reponseHtml = "-1";
            try
            {
                var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);

                tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues);
                TokenResponse tokenResponse = RequestToken(tenant.UserTribunet, pss);
                string accessToken = tokenResponse.AccessToken;
                //The purpose of the refresh token is to retrieve new access token when the ols expires
                string refreshToken = tokenResponse.RefreshToken;
                reponseHtml = CallApi(accessToken, method, tenant, voucherkey,dueDate, path);
                var newTokenResponse = RefreshToken(refreshToken);
            }
            catch (Exception e)
            {
            }
            return reponseHtml;
        }

        public static TokenResponse RequestToken(string Username, string Password)
        {
            //This is call to the token endpoint with the parameters that are set
            TokenResponse tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(Username, Password, Scopes, AdditionalParameters).Result;

            if (tokenResponse.IsError)
            {
                throw new ApplicationException("Couldn't get access token. Error: " + tokenResponse.Error);
            }

            return tokenResponse;
        }

        public static string GetStringFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        /// <summary>
        /// Obtiene el nombre del atributo del XML
        /// </summary>
        /// <param name="itype"></param>
        /// <returns></returns>
        public static string GetXmlAttrName(IdentificacionTypeTipo itype)
        {

            string e = Enum.GetName(typeof(IdentificacionTypeTipo), itype);
            var emisortype = (itype.GetType().GetField(e).GetCustomAttributes(typeof(XmlEnumAttribute), true)[0] as XmlEnumAttribute).Name;

            return emisortype;
        }

        public static string CallApi(string accessToken, string method, Client client, Tenant tenant, Invoice invoice, string voucherkey, Note note, Voucher voucher, bool loadXmlFromAzureStorage = false)
        {
            string param = "";
            HttpWebRequest request;

            if (method == "GET")
            {
                param = "/" + voucherkey;
                request = (HttpWebRequest)WebRequest.Create(WebApiNewsEndPoint + param);
            }
            else
                request = (HttpWebRequest)WebRequest.Create(WebApiNewsEndPoint);

            request.ContentType = "application/json; charset=utf-8";
            request.Method = method;
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            if (method == "POST")
            {
                string fileContents = string.Empty;
                DateTime datedoc = DateTime.Now;

                // si es una NCR o ND
                if (note != null)
                {
                    if (loadXmlFromAzureStorage)
                        fileContents = LoadXmlInvoiceFromAzureStorage(invoice.VoucherKey, invoice.ElectronicBill);
                    else
                        fileContents = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + voucherkey + ".xml"));
                    datedoc = note.CreationTime;
                }
                else
                {   // si es una factura
                    if (voucher!=null)
                    {                       
                        fileContents = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlSignedPath(), "voucher_" + voucherkey + ".xml"));
                        datedoc = voucher.CreationTime;
                    }
                    else
                    {
                        datedoc = invoice.DueDate;
                        if (loadXmlFromAzureStorage)
                            fileContents = LoadXmlInvoiceFromAzureStorage(invoice.VoucherKey, invoice.ElectronicBill);
                        else
                            fileContents = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlSignedPath(), voucherkey + ".xml"));
                    }
                    
                }

                byte[] bytes = Encoding.UTF8.GetBytes(fileContents);
                string docXML = Convert.ToBase64String(bytes);

                var comprobante = (note != null) ? (IComprobanteRecepcion)note : ( (invoice!=null)? (IComprobanteRecepcion)invoice : (IComprobanteRecepcion)voucher);

                ElectronicBill payload = BuildElectronicBill(comprobante, tenant, client, datedoc, docXML);

                var json = JsonConvert.SerializeObject(payload);
                //hace el request
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

            }

            string html = string.Empty;
            try
            {   //obtiene el response
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    html = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                html = "-1";
            }
            return html;
        }
        /// <summary>
        /// Envia Xml 
        /// </summary>
        public static string CallApi(string accessToken, string method, Tenant tenant, string voucherkey, DateTime dueDate, string pathXML, bool loadXmlFromAzureStorage = false)
        {
            string param = "";
            HttpWebRequest request;

            if (method == "GET")
            {
                param = "/" + voucherkey;
                request = (HttpWebRequest)WebRequest.Create(WebApiNewsEndPoint + param);
            }
            else
                request = (HttpWebRequest)WebRequest.Create(WebApiNewsEndPoint);

            request.ContentType = "application/json; charset=utf-8";
            request.Method = method;
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            if (method == "POST")
            {
                string fileContents = string.Empty;

                if (loadXmlFromAzureStorage)
                    fileContents = LoadXmlInvoiceFromAzureStorage(voucherkey, pathXML);
                else
                    fileContents = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlSignedClientPath(), voucherkey + ".xml"));


                byte[] bytes = Encoding.UTF8.GetBytes(fileContents);
                string docXML = Convert.ToBase64String(bytes);

                //var comprobante = (note != null) ? (IComprobanteRecepcion)note : ((invoice != null) ? (IComprobanteRecepcion)invoice : (IComprobanteRecepcion)voucher);

                ElectronicBill payload = BuildElectronicBill(voucherkey, tenant, null, dueDate, docXML);

                var json = JsonConvert.SerializeObject(payload);
                //hace el request
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

            }

            string html = string.Empty;
            try
            {   //obtiene el response
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    html = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                html = "-1";
            }
            return html;
        }

        public static TokenResponse RefreshToken(string refreshToken)
        {
            //This is call to the token endpoint that can retrieve new access and refresh token from the current refresh token
            return tokenClient.RequestRefreshTokenAsync(refreshToken).Result;
        }

        public bool ResendInvoice(IComprobanteRecepcion comprobante, Client client, DateTime datedoc, Tenant tenant, string token)
        {
            if (comprobante == null)
            {
                return false;
            }
            bool result = false;
            //result = CallApi(token, "POST", invoice.Client, invoice.Tenant, invoice, invoice.VoucherKey, null, true) != "-1";
            try
            {
                var electronicBill = BuildElectronicBill(comprobante, client, datedoc, tenant);
                if (electronicBill != null)
                {
                    var response = HttpClientHelper.PostAsJsonAsync<ElectronicBill, object>(ApiComprobanteElectronicoUrl, requestUri, token, electronicBill).Result;
                    result = response.Success;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public ElectronicBillResponse GetComprobanteStatusFromTaxAdministration(IComprobanteRecepcion comprobante, string token)
        {
            ElectronicBillResponse result = null;
            try
            {
                var response = HttpClientHelper.GetAsync<ElectronicBillResponse>(ApiComprobanteElectronicoUrl, requestUri + "/" + comprobante.VoucherKey, token).Result;
                if (response.Success)
                {
                    result = response.Result;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public ElectronicBillResponse GetComprobanteStatusFromTaxAdministrationRefreshToken(IComprobanteRecepcion comprobante, string refreshToken)
        {
            ElectronicBillResponse result = null;
            tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues);
            string token = RefreshToken(refreshToken).AccessToken;
            result = GetComprobanteStatusFromTaxAdministration(comprobante, token);
            return result;
        }

        public ElectronicBillResponse GetComprobanteStatusFromTaxAdministration(IComprobanteRecepcion comprobante, string user, string encriptPassword)
        {
            ElectronicBillResponse result = null;
            var pss = CryptoHelper.Desencriptar(encriptPassword);
            TokenResponse tokenResponse = LoginAsync(user, pss).Result;
            result = GetComprobanteStatusFromTaxAdministration(comprobante, tokenResponse.AccessToken);
            return result;
        }

        private ElectronicBill BuildElectronicBill(IComprobanteRecepcion comprobante,Client client, DateTime datedoc, Tenant tenant)
        {
            if (comprobante == null || comprobante.ElectronicBill == null)
            {
                return null;
            }
           // DateTime datedoc = comprobante.DueDate;
            var fileContents = LoadXmlInvoiceFromAzureStorage(comprobante.VoucherKey, comprobante.ElectronicBill);
            byte[] bytes = Encoding.UTF8.GetBytes(fileContents);
            string docXML = Convert.ToBase64String(bytes);

            // obtiene el atributo tipo del emisor y receptor
            ElectronicBill electronicBill = BuildElectronicBill(comprobante, tenant, client, datedoc, docXML);
            return electronicBill;
        }

        private static ElectronicBill BuildElectronicBill(IComprobanteRecepcion comprobante, Tenant tenant, Client client, DateTime datedoc, string docXML)
        {
            var receptortype = string.Empty;

            var emisortype = GetXmlAttrName(tenant.IdentificationType);

            if (client!=null)
             receptortype = GetXmlAttrName(client.IdentificationType);
            
            var electronicBill = new ElectronicBill
            {
                Numberkey = comprobante.VoucherKey,
                Date = datedoc.ToUniversalTime().ToString("O"),
                Emisor = new Transmitter
                {
                    TypeIdentification = emisortype,
                    IdentificationNumber = tenant.IdentificationNumber
                },
                Receptor = ((client == null)||(client.IdentificationType == IdentificacionTypeTipo.NoAsiganda)) ? null : new Receiver
                {
                    TypeIdentification = receptortype,
                    IdentificationNumber = client.Identification
                },
                Xml = docXML
            };
            return electronicBill;
        }

        private static ElectronicBill BuildElectronicBill(string comprobante, Tenant tenant, Client client, DateTime datedoc, string docXML)
        {
            var receptortype = string.Empty;

            var emisortype = GetXmlAttrName(tenant.IdentificationType);

            if (client != null)
                receptortype = GetXmlAttrName(client.IdentificationType);

            var electronicBill = new ElectronicBill
            {
                Numberkey = comprobante,
                Date = datedoc.ToUniversalTime().ToString("O"),
                Emisor = new Transmitter
                {
                    TypeIdentification = emisortype,
                    IdentificationNumber = tenant.IdentificationNumber
                },
                Receptor = ((client == null) || (client.IdentificationType == IdentificacionTypeTipo.NoAsiganda)) ? null : new Receiver
                {
                    TypeIdentification = receptortype,
                    IdentificationNumber = client.Identification
                },
                Xml = docXML
            };
            return electronicBill;
        }

        private static string LoadXmlInvoiceFromAzureStorage(string voucherKey, string electronicBill)
        {
            string xml = string.Empty;
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference("xmlinvoice");
                if (blobContainer.Exists())
                {
                    var docName = electronicBill.Replace((blobContainer.Uri.OriginalString.Contains(":443")) ? blobContainer.Uri.OriginalString.Replace(":443", "") : blobContainer.Uri.OriginalString, "").Replace("/", "");
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(docName);
                    xml = blockBlob.DownloadText();
                }
            }
            catch (Exception)
            {
            }
            return xml;
        }

        public async Task<TokenResponse> LoginAsync(string Username, string Password)
        {
            TokenResponse tokenResponse = null;
            try
            {
                using (var tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues))
                {
                    tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(Username, Password, Scopes, AdditionalParameters).ConfigureAwait(false); 
                }
            }
            catch (Exception)
            {
            }
            return tokenResponse;
        }

        /// <summary>
        /// Envia la factura electronica al ente regulador
        /// </summary>
        /// <param name="tenant">Tenant de la factura</param>
        /// <param name="client">Cliente de la factura</param>
        /// <param name="invoice">Factura a ser enviada</param>
        /// <returns></returns>
        //public async Task SendInvoiceAsync(Tenant tenant, Client client, Invoice invoice)
        //{
        //    var authResult = await LoginAsync();
        //    if (authResult != null && !authResult.IsError)
        //    {
        //        ElectronicBill payload = InvoiceToElectronicBill(invoice);
        //        var result = await HttpClientHelper.PostAsJsonAsync<ElectronicBill, object>(ApiComprobanteElectronicoUrl, "recepcion", authResult.AccessToken, payload);
        //    }
        //}

        //private ElectronicBill InvoiceToElectronicBill(Invoice invoice)
        //{
        //    //******************************************///
        //    // Los datos que se estan enviando no son validos, 
        //    // el problema no esta en el formato, esta en los datos (ElectronicBill).
        //    //   .quizas el emisor y receptor no pueden ser los mismo
        //    //   .quizas los numeros de identificacion que se envian no son validos
        //    //   .quizas la firma aplicada al xml no esta correcta.
        //    //******************************************///
        //    string docXML = GetInvoiceXml(invoice.VoucherKey);
        //    var payload = new ElectronicBill
        //    {
        //        Numberkey = invoice.VoucherKey,
        //        Date = invoice.DueDate.ToUniversalTime().ToString("O"),
        //        Emisor = new Transmitter
        //        {
        //            TypeIdentification = "02",
        //            IdentificationNumber = "003101123456"
        //        },
        //        Receptor = new Receiver
        //        {
        //            TypeIdentification = "02",
        //            IdentificationNumber = "003101123456"
        //        },
        //        Xml = docXML
        //    };
        //    var testJson = JsonConvert.SerializeObject(payload);//TODO:para probar el json, borrar al finalizar
        //    return payload;
        //}

        //private string GetInvoiceXml(string voucherKey)
        //{
        //    string invoicePath = Path.Combine(WorkPaths.GetXmlSignedPath(), voucherKey + ".xml");
        //    if (!File.Exists(invoicePath))
        //    {
        //        return string.Empty;
        //    }

        //    string docXML = string.Empty;
        //    try
        //    {
        //        using (var reader = new StreamReader(invoicePath))
        //        {
        //            string fileContents = reader.ReadToEnd();
        //            byte[] bytes = Encoding.UTF8.GetBytes(fileContents);
        //            docXML = Convert.ToBase64String(bytes);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return docXML;
        //}
    }

    public class ElectronicBill
    {
        [JsonProperty("clave")]
        public string Numberkey { get; set; }

        [JsonProperty("fecha")]
        public string Date { get; set; }

        [JsonProperty("emisor")]
        public Transmitter Emisor { get; set; }

        [JsonProperty("receptor", NullValueHandling = NullValueHandling.Ignore)]
        public Receiver Receptor { get; set; }

        [JsonProperty("comprobanteXml")]
        public string Xml { get; set; }

        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
    }

    public class Transmitter
    {
        [JsonProperty("tipoIdentificacion")]
        public string TypeIdentification { get; set; }

        [JsonProperty("numeroIdentificacion")]
        public string IdentificationNumber { get; set; }
    }

    public class Receiver
    {
        [JsonProperty("tipoIdentificacion")]
        public string TypeIdentification { get; set; }

        [JsonProperty("numeroIdentificacion")]
        public string IdentificationNumber { get; set; }
    }

    public class ElectronicBillResponse
    {
        [JsonProperty("clave")]
        public string Clave { get; set; }

        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("ind-estado")]
        public string IndEstado { get; set; }

        [JsonProperty("respuesta-xml")]
        public string RespuestaXml { get; set; }
    }
}
