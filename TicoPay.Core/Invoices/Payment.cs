using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices.XSD;

namespace TicoPay.Invoices
{
    public class Payment : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        public Guid? ClientId { get; set; }

        public virtual Client Client { get; protected set; }
        /// <summary>
        /// Gets or sets the Amount. 
        /// </summary>
        public decimal Amount { get; protected set; }

        public decimal Balance { get; set; }

        public DateTime PaymentDate { get; set; }       

        /// <summary>Gets or sets the ExchangeRate Id. </summary>
        public Guid? ExchangeRateId { get; protected set; }

        /// <summary>Gets or sets the ExchangeRate. </summary>
        public virtual ExchangeRate ExchangeRate { get; protected set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }        

        /// <summary>Gets or sets the PaymentInvoiceType. </summary>
        public PaymentType PaymentType { get; set; }

        public PaymentOrigin PaymentOrigin { get; set; }

        public PaymetnMethodType PaymetnMethodType { get; set; }

        public string Transaction { get; set; }

        /// <summary>Gets or sets the Reference. Ex: voucher number </summary>
        public string Reference { get; set; }

        public int? CodigoBanco { get; set; }

        public int? CodigoAgencia { get; set; }

        public int? CodigoTransaccion { get; set; }

        public int? ConsecutivoTransaccion { get; set; }

        public int? NotaCredito { get; set; }

        public int? CodigoBancoEmisor { get; set; }

        public int? NumeroCuenta { get; set; }

        public int? NumeroCheque { get; set; }      

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public bool? IsPaymentReversed { get; set; }

        public bool? IsPaymentUsed { get; set; }

        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid? ParentPaymentInvoiceId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<PaymentInvoice> PaymentInvoices { get; protected set; }

        public virtual ICollection<PaymentNote> PaymentNotes { get; protected set; }


        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected Payment()
        {

        }

        public static Payment Create(int tenantId, Client client, decimal amount,  PaymentType paymentType, string reference,
            FacturaElectronicaResumenFacturaCodigoMoneda codigomoneda, PaymentOrigin paymentOrigin)
        {
            var entity = new Payment
            {
                Id = Guid.NewGuid(),               
                TenantId = tenantId,
                Amount = amount,      
                Balance= amount,
                PaymentType = paymentType,
                Reference = reference,
                PaymentDate = DateTimeZone.Now(),
                CodigoMoneda = codigomoneda,
                PaymentOrigin = paymentOrigin
            };

            if (client != null)
            {
                entity.ClientId = client.Id;
                entity.Client = client;
            }             
                
            entity.PaymentInvoices = new Collection<PaymentInvoice>();
            entity.PaymentNotes = new Collection<PaymentNote>();
            return entity;
        }

        public void SetPaymetnMethodType(PaymetnMethodType Type) => PaymetnMethodType = Type;


        public void SetTransaction(string trans) => Transaction = trans;

        public void AssignInvoice(int tenantId, decimal amount, Invoice invoice, PaymetnMethodType paymetnMethodType, Guid? bankId, string userCard)
        {
            if (amount> Balance)
            {
                throw new UserFriendlyException("El monto a aplicar a la factura no puede ser mayor al saldo del Pago.");
            }
            Balance = Balance - amount;
            var paymentInvoice = PaymentInvoice.Create(TenantId, amount, invoice, paymetnMethodType, bankId, userCard);
            PaymentInvoices.Add(paymentInvoice);
        }
        public void AssignNote(int tenantId, decimal amount, Note Note, PaymetnMethodType paymetnMethodType)
        {
            if (amount > Balance)
            {
                throw new UserFriendlyException("El monto a aplicar a la Nota no puede ser mayor al saldo del Pago.");
            }
            Balance = Balance - amount;
            var paymentNote = PaymentNote.Create(TenantId, amount, Note, paymetnMethodType);
            PaymentNotes.Add(paymentNote);
        }

    }

    public enum PaymentOrigin
    {
        [Description("Ticopay")]
        ticopay = 1, 
        [Description("Banco Nacional")]
        BNpay
    }

    /// <summary>
    /// Enumerado que contiene los tipos de Pago / Enum that contains the Payment Types
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Pago / Payment
        /// </summary>
        [Description("Pago")]
        Payment,
        /// <summary>
        /// Devolución / Refund
        /// </summary>
        [Description("Devolución")]
        Refund
    }
}

