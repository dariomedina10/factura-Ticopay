using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public class ErrorCodeHelper
    {
        public static int Ok = 1;
        public static int None = 0;
        public static int Error = -1;
        public static int OkMessage = 2;
        public static int InvoicesMonthlyLimitReached = -2;
        public static int CantCreateInvoices = -3;
        public static int InvoicesWithoutFirm = -4;
        public static int EditionHasNotApiAccess = -5;
        public static int TenantAddressIsNotComplete = -6;
        public static int TenantHasNotHaciendaCredential = -7;
    }
}
