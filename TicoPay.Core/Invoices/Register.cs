using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using TicoPay.Drawers;

namespace TicoPay.Invoices
{
    public class Register : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        [MaxLength(60)]
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get;  set; }

        [MaxLength(5)]
        public string RegisterCode { get; set; }
        /// <summary>
        /// Gets or sets the Invoice Number
        /// </summary>
        public long FirstInvoiceNumber { get; set; }
        public long LastInvoiceNumber { get; set; }

        public long FirstNoteDebitNumber { get; set; }
        public long LastNoteDebitNumber { get; set; }

        public long FirstNoteCreditNumber { get; set; }
        public long LastNoteCreditNumber { get; set; }


        public long FirstVoucherNumber { get; set; }
        public long LastVoucherNumber { get; set; }

        public long FirstTicketNumber { get; set; }
        public long LastTicketNumber { get; set; }

        [MaxLength(10)]
        /// <summary>
        /// Gets or sets the Invoice Prefix
        /// </summary>
        public string InvoicePrefix { get; protected set; }

        [MaxLength(10)]
        /// <summary>
        /// Gets or sets the Invoice Prefix
        /// </summary>
        public string InvoiceSufix { get; protected set; }

        /// <summary>
        /// Gets or sets the Show Discount On Invoice
        /// </summary>
        public bool ShowDiscountOnInvoice { get; protected set; }

        /// <summary>
        /// Gets or sets the ask for a note on save, layby, account, return
        /// </summary>
        public bool AskForANoteOnSLAR { get; protected set; }

        /// <summary>
        /// Gets or sets the ask for a note on all sales
        /// </summary>
        public bool AskForANoteOnAllSales { get; protected set; }

        /// <summary>
        /// Gets or sets the print invoice
        /// </summary>
        public bool PrintInvoice { get; protected set; }

        /// <summary>
        /// Gets or sets the email invoice
        /// </summary>
        public bool EmailInvoice { get; protected set; }

        /// <summary>
        /// Gets or sets the Select User For Next Sale
        /// </summary>
        public bool SelectUserForNextSale { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public Guid?  DrawerId { get; set; }

        public virtual Drawer Drawer { get; set; }
        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected Register()

        {
        }

        public static Register Create(int tenantId, string name, string registerCode, long firstInvoiceNumber, long lastInvoiceNumber,
            long firstNoteDebitNumber, long lastNoteDebitNumber, long firstNoteCreditNumber, long lastNoteCreditNumber,  long firstVoucherNumber, long lastVoucherNumber,
        string invoicePrefix, string invoiceSufix, bool showDiscountOnInvoice, bool askForANoteOnSLAR, bool askForANoteOnAllSales,
            bool printInvoice, bool emailInvoice, bool selectUserForNextSale, Guid drawerId)
        {
            return new Register
            {
                TenantId = tenantId,
                Name = name,
                RegisterCode = registerCode,
                FirstInvoiceNumber = firstInvoiceNumber,
                LastInvoiceNumber = lastInvoiceNumber,
                FirstNoteDebitNumber = firstNoteDebitNumber,
                LastNoteDebitNumber = lastNoteDebitNumber,
                FirstNoteCreditNumber = firstNoteCreditNumber,
                LastNoteCreditNumber = lastNoteCreditNumber,
                FirstVoucherNumber=firstVoucherNumber,
                LastVoucherNumber=lastVoucherNumber,
                InvoicePrefix = invoicePrefix,
                InvoiceSufix = invoiceSufix,
                ShowDiscountOnInvoice = showDiscountOnInvoice,
                AskForANoteOnSLAR = askForANoteOnSLAR,
                AskForANoteOnAllSales = askForANoteOnAllSales,
                PrintInvoice = printInvoice,
                EmailInvoice = emailInvoice,
                SelectUserForNextSale = selectUserForNextSale,
                DrawerId= drawerId
            };
        }
    }
}
