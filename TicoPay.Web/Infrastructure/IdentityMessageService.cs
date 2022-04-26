using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SendMail;

namespace TicoPay.Web.Infrastructure
{
    public class IdentityMessageService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            SendMailTP mail = new SendMailTP();
            await mail.SendNoReplyMailAsync(new string[] { message.Destination }, message.Subject, message.Body);
        }
    }
}