using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace TicoPay.Invoices
{
    public class PaymentInvoice : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the Amount. 
        /// </summary>
        public decimal Amount { get; protected set; }
               
        public PaymetnMethodType PaymetnMethodType { get; set; }

        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid InvoiceId { get; protected set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public virtual Invoice Invoice { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public bool? IsPaymentReversed { get; set; }

        public DateTime? DeletionTime { get; set; }

        public Guid PaymentId { get; protected set; }

        public virtual Payment Payment { get; protected set; }

        public virtual Bank Bank { get; set; }

        public Guid? BankId { get; set; }

        [StringLength(60)]
        public string UserCard { get; set; }

        ///// <summary>
        ///// gets or sets Date payment Invoice
        ///// </summary>
        //public DateTime PaymentDate { get; set; }

        // /// <summary>Gets or sets the ExchangeRate Id. </summary>
        //public Guid? ExchangeRateId { get; protected set; }

        ///// <summary>Gets or sets the ExchangeRate. </summary>
        //public virtual ExchangeRate ExchangeRate { get; protected set; }

        //public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        /////// <summary>Gets or sets the PaymentMethod. </summary>
        ////public virtual PaymentMethod PaymentMethod { get; protected set; }

        /////// <summary>Gets or sets the PaymentMethod id. </summary>
        ////public Guid PaymentMethodId { get; protected set; }

        ///// <summary>Gets or sets the PaymentInvoiceType. </summary>
        //public PaymentType PaymentInvoiceType { get; set; }

        //public PaymetnMethodType PaymetnMethodType { get; set; }

        //public string Transaction { get; set; }

        ///// <summary>Gets or sets the Reference. Ex: voucher number </summary>
        //public string Reference { get; set; }

        //public int? CodigoBanco { get; set; }
        //public int? CodigoAgencia { get; set; }
        //public int? CodigoTransaccion { get; set; }
        //public int? ConsecutivoTransaccion { get; set; }
        //public int? NotaCredito { get; set; }
        //public int? CodigoBancoEmisor { get; set; }
        //public int? NumeroCuenta { get; set; }
        //public int? NumeroCheque { get; set; }

        // /// <summary>Gets or sets the CurrencyCode. </summary>
        //public string CurrencyCode { get; protected set; }

        //public Guid? UserId { get; set; }

        ///// <summary>Gets or sets the User Name. </summary>
        //public string UserName { get; set; }



        //public bool? IsPaymentUsed { get; set; }

        ///// <summary>Gets or sets the InvoiceId. </summary>
        //public Guid? ParentPaymentInvoiceId { get; set; }



        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        public PaymentInvoice()
        {

        }

        public static PaymentInvoice Create(int tenantId, decimal amount, Invoice invoice, PaymetnMethodType paymetnMethodType, Guid? bankId, string userCard)
        {
            var entity = new PaymentInvoice
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Amount = amount,
                IsPaymentReversed = false,           
                Invoice = invoice,
                PaymetnMethodType = paymetnMethodType,
                BankId = bankId,
                UserCard = userCard
            };
            return entity;
        }

        //public void PayInvoiceCash()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Cash;

        //}
        //public void PayInvoiceCreditCard()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Card;
        //}
        //public void PayInvoiceCheck()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Check;
        //}
        //public void PayInvoiceDeposit()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Deposit;
        //}
        //public void PayInvoicePositiveBalance()
        //{
        //    PaymetnMethodType = PaymetnMethodType.PositiveBalance;
        //}


        //public void SetTransaction(string trans)
        //{
        //    Transaction = trans;
        //}

        //public void SetUserNamePayment(string username)
        //{
        //    UserName = username;
        //}

    }

    
}
