using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using SendGrid;
using System.Net;
using System.Configuration;
using System.IO;
using System.Collections;

namespace SendMail
{
    public class SendMailTP
    {
        public void SendMailTicoPay(string _email, string _subject, string _emailbody, string _emailfooter, string _urltoken, string emailalternative, string _remitente,string _cuentaBanca)
        {
            SendGridMessage mm = new SendGridMessage();
            StringBuilder sb = new StringBuilder();

         
            try

            {
                sb.Append("<p><b>Hola</b> " + _email + "</p>");
                sb.Append("<p>La Empresa <b>" + _remitente + "</b> te ha enviado el siguiente documento</p>");
                sb.Append(_emailbody);
                sb.Append(_urltoken);
                sb.Append(_cuentaBanca);
                sb.Append(_emailfooter);
                sb.Append("<p>Documento Generado por medio de: https://www.ticopays.com/</p>");

                mm.Subject = _subject;
                mm.From = new MailAddress("noreply@ticopays.com", "TicoPay Team");
                mm.Html = sb.ToString();

                List<string> ToEmailAddress = new List<string>();
                ToEmailAddress.Add(_email);
                if ((emailalternative != null) && (emailalternative != string.Empty))
                    ToEmailAddress.Add(emailalternative);
                mm.AddTo(ToEmailAddress);


                //if ((emailalternative != null) && (emailalternative != string.Empty))
                //    mm.AddBcc(emailalternative);

                //DELIVER == ENVIO SEGURO
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["Mail.Account"], ConfigurationManager.AppSettings["Mail.Password"]);
                var transportWeb = new Web(credentials);
                transportWeb.Deliver(mm);
            }
            catch (Exception e)
            {

            }

           

        }

        public void SendMailTicoPay(string _email, string _subject, string _emailbody, string _emailfooter, string _urltoken, string rutaXML, string rutaPDF, string qrcode, string rutaXMLSigned, string emailalternative, string _remitente, string _cuentaBanca, string emailCopy)
        {
            SendGridMessage mm = new SendGridMessage();
            StringBuilder sb = new StringBuilder();

            FileInfo xml;
            FileInfo xmlSigned = null;
            FileInfo pdf = null;
            FileInfo qr;

            try

            {
                sb.Append("<p><b>Hola</b> " + _email + "</p>");
                sb.Append("<p>La Empresa <b>" + _remitente + "</b> te ha enviado el siguiente documento</p>");
                sb.Append(_emailbody);
                sb.Append(_urltoken);
                sb.Append(_cuentaBanca);
                sb.Append(_emailfooter);
                sb.Append("<p>Documento Generado por medio de: https://www.ticopays.com/</p>");


                mm.Subject = _subject;
                mm.From = new MailAddress("noreply@ticopays.com", "TicoPay Team");
                mm.Html = sb.ToString();

                List<string> ToEmailAddress = new List<string>();
                ToEmailAddress.Add(_email);
                if ((emailalternative != null) && (emailalternative != string.Empty))
                    ToEmailAddress.Add(emailalternative);
                mm.AddTo(ToEmailAddress);

                

                if ((emailCopy != null) && (emailCopy != string.Empty))
                {
                    string emails = emailCopy;
                    string[] email;
                    email = emails.Split(',');
                    foreach (var item in email)
                    {
                        ToEmailAddress.Add(item);
                        mm.AddTo(ToEmailAddress);
                    }
                }
                    

                if (@rutaPDF != string.Empty)
                {
                    pdf = new FileInfo(@rutaPDF);
                    mm.AddAttachment(@rutaPDF);
                }

                if (@rutaXMLSigned != string.Empty)
                {
                    xmlSigned = new FileInfo(@rutaXMLSigned);
                    mm.AddAttachment(@rutaXMLSigned);
                }

                //if ((emailalternative != null) && (emailalternative != string.Empty))
                //    mm.AddBcc(emailalternative);

                //DELIVER == ENVIO SEGURO
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["Mail.Account"], ConfigurationManager.AppSettings["Mail.Password"]);
                var transportWeb = new Web(credentials);
                transportWeb.Deliver(mm);
            }
            catch (Exception e)
            {

            }

            //elimino los archivos
            if (string.IsNullOrEmpty(@rutaXMLSigned))
            {
                try { xmlSigned.Delete(); } catch (IOException) { }
            }
            if (string.IsNullOrEmpty(@rutaPDF))
            {
                try { pdf.Delete(); } catch (IOException) { }
            }
            if (string.IsNullOrEmpty(@rutaXML))
            {
                xml = new FileInfo(@rutaXML);
                try { xml.Delete(); } catch (IOException) { }
            }
            if (string.IsNullOrEmpty(@qrcode))
            {
                qr = new FileInfo(@qrcode);
                try { qr.Delete(); } catch (IOException) { }
            }

        }

        public void SendMailTicoPay(string _email, string _subject, string _emailbody, string _emailfooter, string _urltoken,  string rutaXMLSigned, string emailalternative, string _remitente,string _cuentaBanca)
        {
            SendGridMessage mm = new SendGridMessage();
            StringBuilder sb = new StringBuilder();

           
            FileInfo xmlSigned = null;
           

            try

            {
                sb.Append("<p><b>Hola</b> " + _email + "</p>");
                sb.Append(_emailbody);
                sb.Append(_urltoken);
                sb.Append(_cuentaBanca);
                sb.Append(_emailfooter);


                mm.Subject = _subject;
                mm.From = new MailAddress("noreply@ticopays.com", "TicoPay Team");
                mm.Html = sb.ToString();

                List<string> ToEmailAddress = new List<string>();
                ToEmailAddress.Add(_email);
                if ((emailalternative != null) && (emailalternative != string.Empty))
                    ToEmailAddress.Add(emailalternative);
                mm.AddTo(ToEmailAddress);

                if (@rutaXMLSigned != string.Empty)
                {
                    xmlSigned = new FileInfo(@rutaXMLSigned);
                    mm.AddAttachment(@rutaXMLSigned);
                }

                //if ((emailalternative != null) && (emailalternative != string.Empty))
                //    mm.AddBcc(emailalternative);

                //DELIVER == ENVIO SEGURO
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["Mail.Account"], ConfigurationManager.AppSettings["Mail.Password"]);
                var transportWeb = new Web(credentials);
                transportWeb.Deliver(mm);
            }
            catch (Exception e)
            {

            }

            //elimino los archivos
            if (string.IsNullOrEmpty(@rutaXMLSigned))
            {
                try { xmlSigned.Delete(); } catch (IOException) { }
            }
           

        }

        public void SendMailTicoPay(string email, string subject, string emailbody, string emailfooter, string urltoken, Stream pdf, string pdfFileName, Stream xmlSigned, string xmlFileName, string _remitente,string _cuentaBanca)
        {
            SendGridMessage message = new SendGridMessage();

            StringBuilder body = new StringBuilder();
            body.Append("<p><b>Hola</b> " + email + "</p>");
            body.Append("<p>La Empresa <b>" + _remitente + "</b> te ha enviado el siguiente documento</p>");
            body.Append(emailbody);
            body.Append(urltoken);
            body.Append(_cuentaBanca);
            body.Append(emailfooter);
            body.Append("<p>Documento Generado por medio de: https://www.ticopays.com/</p>");

            message.Subject = subject;
            message.From = new MailAddress("noreply@ticopays.com", "TicoPay Team");
            message.Html = body.ToString();
            message.AddTo(email);
            message.AddAttachment(pdf, pdfFileName);
            message.AddAttachment(xmlSigned, xmlFileName);

            try
            {
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["Mail.Account"], ConfigurationManager.AppSettings["Mail.Password"]);
                var transportWeb = new Web(credentials);
                transportWeb.Deliver(message);
            }
            catch (Exception)
            {
            }
        }

        public async Task SendNoReplyMailAsync(string[] to, string subject, string body, string[] attachmentFilePath = null, string[] embedImageFilePaths = null)
        {
            await SendMailAsync(new MailAddress("noreply@ticopays.com", "TicoPay Team"), to, subject, body, attachmentFilePath, embedImageFilePaths);
        }

        public async Task SendMailAsync(MailAddress from, string[] to, string subject, string body, string[] attachmentFilePath = null, string[] embedImageFilePaths = null)
        {
            SendGridMessage mail = new SendGridMessage();
            mail.From = from;
            mail.Subject = subject;
            mail.Html = body;
            mail.Text = body;
            foreach (string email in to)
            {
                if (!string.IsNullOrWhiteSpace(email))
                    mail.AddTo(email);
            }
            if (attachmentFilePath != null)
            {
                foreach (string filePath in attachmentFilePath)
                {
                    if (!string.IsNullOrWhiteSpace(filePath))
                        mail.AddAttachment(filePath);
                }
            }
            if (embedImageFilePaths != null)
            {
                foreach (var fileName in embedImageFilePaths)
                {
                    string cid = Path.GetFileNameWithoutExtension(fileName);
                    mail.EmbedImage(fileName, cid);
                }
            }

            try
            {
                string account = ConfigurationManager.AppSettings["Mail.Account"];
                string password = ConfigurationManager.AppSettings["Mail.Password"];
                var transportWeb = new Web(new NetworkCredential(account, password));
                await transportWeb.DeliverAsync(mail);
            }
            catch (Exception)
            {
            }
        }

        public void SendNoReplyMail(string[] to, string subject, string body, string[] attachmentFilePath = null, string[] embedImageFilePaths = null)
        {
            SendMail(new MailAddress("noreply@ticopays.com", "TicoPay Team"), to, subject, body, attachmentFilePath, embedImageFilePaths);
        }

        public void SendMail(MailAddress from, string[] to, string subject, string body, string[] attachmentFilePath = null, string[] embedImageFilePaths = null)
        {
            SendGridMessage mail = new SendGridMessage();
            mail.From = from;
            mail.Subject = subject;
            mail.Html = body;
            mail.Text = body;
            foreach (string email in to)
            {
                if (!string.IsNullOrWhiteSpace(email))
                    mail.AddTo(email);
            }
            if (attachmentFilePath != null)
            {
                foreach (string filePath in attachmentFilePath)
                {
                    if (!string.IsNullOrWhiteSpace(filePath))
                        mail.AddAttachment(filePath);
                }
            }
            if (embedImageFilePaths != null)
            {
                foreach (var fileName in embedImageFilePaths)
                {
                    string cid = Path.GetFileNameWithoutExtension(fileName);
                    mail.EmbedImage(fileName, cid);
                }
            }

            try
            {
                string account = ConfigurationManager.AppSettings["Mail.Account"];
                string password = ConfigurationManager.AppSettings["Mail.Password"];
                var transportWeb = new Web(new NetworkCredential(account, password));
                transportWeb.Deliver(mail);
            }
            catch (Exception)
            {
            }
        }
    }
}
