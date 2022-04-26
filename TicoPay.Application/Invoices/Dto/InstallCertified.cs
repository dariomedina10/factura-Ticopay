                             using SendMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TicoPay.MultiTenancy.Dto;

namespace TicoPay.Invoices.Dto
{
    public class InstallCertified
    {
        private static bool InstallCertificate(string certificatePath, string certificatePassword)
        {
            try
            {
                var serviceRuntimeUserCertificateStore = new X509Store(StoreName.Root);
                serviceRuntimeUserCertificateStore.Open(OpenFlags.ReadWrite);

                X509Certificate2 cert = null;

                try
                {
                    cert = new X509Certificate2(certificatePath, certificatePassword);
                }
                catch (Exception)                                                      
                {
                    return false;
                }

                serviceRuntimeUserCertificateStore.Add(cert);
                serviceRuntimeUserCertificateStore.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GetCertificate(CertifiedTenantOutput _certifiedTenant)
        {
            string _certifiedPath = string.Empty;

            try
            {

                //Guarda el certificado temporalmente

                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + _certifiedTenant.FileName).ToLower();

                _certifiedPath = Path.GetFullPath("~/Common/" + archivo);

                File.WriteAllBytes(_certifiedPath, _certifiedTenant.CertifiedTenant);
            }
            catch (Exception)
            {
                // borra el certificado del directorio temporal
                FileInfo filecert = new FileInfo(_certifiedPath);
                filecert.Delete();
            }
        }
        public async Task InstalarCertificado(CertifiedTenantOutput _certifiedTenant, string emailTenant)
        {
            string _certifiedPath = string.Empty;
            SendMailTP mail = new SendMailTP(); // clase para envio de correo

            try
            {
                //X509Store serviceRuntimeUserCertificateStore = new X509Store(StoreName.Root);
                //string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                //string certificateFolder = "c:\\";
                //string certPath = Path.Combine(baseDir, certificateFolder);

                //string certificateFile = "user-calist.cer";
                //string certificatePassword = "somePassword";
                //string certificateLocation = certPath + certificateFile

                //Guarda el certificado temporalmente

                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + _certifiedTenant.FileName).ToLower();

                _certifiedPath = Path.GetFullPath("~/Uploads/" + archivo);

                File.WriteAllBytes(_certifiedPath, _certifiedTenant.CertifiedTenant);

                //envia correo sino fue satisfactoria la instalacion
                if (!InstallCertificate(_certifiedPath, _certifiedTenant.Password))
                {
                    StringBuilder body = new StringBuilder();
                    body.AppendLine("<div>");
                    body.AppendLine("<p><b>Hola</b> " + emailTenant + "</p>");
                    body.AppendLine("< p > Queremos informale que se presentaron inconvenientes durante la instalación del certificado para la firma digital. Póngase en contacto con nuestro equipo a la dirección de correo que enviamos en este e-mail.</ p >");
                    body.AppendLine("<p> Por favor contáctenos a, soporte@ticopays.com.</ p > < p > Gracias </ p > < p >< b > Equipo TicoPay </ b ></ p >");
                    body.AppendLine("</div>");

                    await mail.SendNoReplyMailAsync(new string[] { emailTenant }, "Instalación de Certificado", body.ToString());
                }

                // borra el certificado del directorio temporal
                FileInfo filecert = new FileInfo(_certifiedPath);
                filecert.Delete();
            }
            catch (Exception)
            {
                //enviar correo si hubo error instalando
                StringBuilder body = new StringBuilder();
                body.AppendLine("<div>");
                body.AppendLine("<p><b>Hola</b> " + emailTenant + "</p>");
                body.AppendLine("< p > Queremos informale que se presentaron inconvenientes durante la instalación del certificado para la firma digital. Póngase en contacto con nuestro equipo a la dirección de correo que enviamos en este e-mail.</ p >");
                body.AppendLine("<p> Por favor contáctenos a, soporte@ticopays.com.</ p > < p > Gracias </ p > < p >< b > Equipo TicoPay </ b ></ p >");
                body.AppendLine("</div>");

                await mail.SendNoReplyMailAsync(new string[] { emailTenant }, "Instalación de Certificado", body.ToString());

                // borra el certificado del directorio temporal
                FileInfo filecert = new FileInfo(_certifiedPath);
                filecert.Delete();
            }
        }
        public async Task SaveAndInstallCertificate (CertifiedTenantOutput _certifiedTenant, string emailTenant)
        {
            string _certifiedPath = string.Empty;
            SendMailTP mail = new SendMailTP(); // clase para envio de correo

            try
            {
                //X509Store serviceRuntimeUserCertificateStore = new X509Store(StoreName.Root);
                //string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                //string certificateFolder = "c:\\";
                //string certPath = Path.Combine(baseDir, certificateFolder);

                //string certificateFile = "user-calist.cer";
                //string certificatePassword = "somePassword";
                //string certificateLocation = certPath + certificateFile

                //Guarda el certificado temporalmente

                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + _certifiedTenant.FileName).ToLower();

                _certifiedPath = Path.GetFullPath("~/Uploads/" + archivo);

                File.WriteAllBytes(_certifiedPath, _certifiedTenant.CertifiedTenant);

                //envia correo sino fue satisfactoria la instalacion
                if (!InstallCertificate(_certifiedPath, _certifiedTenant.Password))
                {
                    StringBuilder body = new StringBuilder();
                    body.AppendLine("<div>");
                    body.AppendLine("<p><b>Hola</b> " + emailTenant + "</p>");
                    body.AppendLine("< p > Queremos informale que se presentaron inconvenientes durante la instalación del certificado para la firma digital. Póngase en contacto con nuestro equipo a la dirección de correo que enviamos en este e-mail.</ p >");
                    body.AppendLine("<p> Por favor contáctenos a, soporte@ticopays.com.</ p > < p > Gracias </ p > < p >< b > Equipo TicoPay </ b ></ p >");
                    body.AppendLine("</div>");

                    await mail.SendNoReplyMailAsync(new string[] { emailTenant }, "Instalación de Certificado", body.ToString());
                }

                // borra el certificado del directorio temporal
                FileInfo filecert = new FileInfo(_certifiedPath);
                filecert.Delete();
            }
            catch (Exception)
            {
                //enviar correo si hubo error instalando
                StringBuilder body = new StringBuilder();
                body.AppendLine("<div>");
                body.AppendLine("<p><b>Hola</b> " + emailTenant + "</p>");
                body.AppendLine("< p > Queremos informale que se presentaron inconvenientes durante la instalación del certificado para la firma digital. Póngase en contacto con nuestro equipo a la dirección de correo que enviamos en este e-mail.</ p >");
                body.AppendLine("<p> Por favor contáctenos a, soporte@ticopays.com.</ p > < p > Gracias </ p > < p >< b > Equipo TicoPay </ b ></ p >");
                body.AppendLine("</div>");

                await mail.SendNoReplyMailAsync(new string[] { emailTenant }, "Instalación de Certificado", body.ToString());

                // borra el certificado del directorio temporal
                FileInfo filecert = new FileInfo(_certifiedPath);
                filecert.Delete();
            }
        }
    }
}
