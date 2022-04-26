using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicoPay.Web.Models.Account
{
    public class SendEmailInput
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string From { get; set; }
    }
}