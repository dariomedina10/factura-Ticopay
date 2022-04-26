using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.General;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.Users;

namespace TicoPay.Banks.Dto
{
    /// <summary>
    /// Clase que contiene la información de un Impuesto
    /// </summary>
    [AutoMapFrom(typeof(Invoices.Bank))]
    public class BankDto : EntityDto<Guid>
    {
        /// <value>
        /// Nombre del Banco.
        /// </value>
        [Required]
        public string Name { get; set; }

        /// <value>
        /// Nombre corto del Banco.
        /// </value>
        [Required]
        public string ShortName { get; set; }

        /// <summary>
        /// bank Is Active
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// gets o sets TennantId
        /// </summary>
        public int TenantId { get; set; }
        /// <summary>
        /// InvoicePaymentMethod
        /// </summary>
        public virtual ICollection<PaymentInvoice> PaymentsInvoices { get; protected set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// DeletedUserId
        /// </summary>
        public long? DeleterUserId { get; set; }
        /// <summary>
        /// DeletedTime
        /// </summary>
        public DateTime? DeletionTime { get; set; }


    }
}
