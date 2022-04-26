using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.EntityFramework;
using TicoPay.Invoices;

namespace TicoPay.Migrations.SeedData
{
    public class UpdatePaymentOrigin
    {
        private readonly TicoPayDbContext _context;

        public UpdatePaymentOrigin(TicoPayDbContext context)
        {
            _context = context;
        }

        public void UpdateByPaymentOrigin()
        {
            var connectionAgrementsTenantId = _context.AgreementsConectivities.Select(a => a.TenantID).ToArray();
            var paymentList = _context.Payments.Where(p => connectionAgrementsTenantId.Any(s => s == p.TenantId));

            foreach (Payment payment in paymentList)
            {
                if (payment.Reference.Equals("Pago por TicoPay"))
                {
                    payment.PaymentOrigin = PaymentOrigin.ticopay;
                }
                else
                {
                    payment.PaymentOrigin = PaymentOrigin.BNpay;
                }
            }
            _context.SaveChanges();
        }
    }
}
