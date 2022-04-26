using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;

namespace SendGridMail
{
    public class SendEmail
    {

        public void SendMailGeneral()
        {
            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress("vfiguera@asadacloud.com", "To Name"));

            // From
            mailMsg.From = new MailAddress("from@example.com", "From Name");

            // Subject and multipart/alternative Body
            mailMsg.Subject = "subject";
            string text = "text body";
            string html = @"<p>html body</p>";
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_2826d192b09b95a6407f906de9998a26@azure.com", "m8PuMwgPIwlG3L4");
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);


        }





    }
}
