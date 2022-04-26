using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TicoPay.Invoices.Dto
{
    /// <summary>
    /// Clase que contiene la información de los medios de pago / Contains the Payment Methods information
    /// </summary>
    [AutoMapFrom(typeof(PaymentInvoice))]
    public class PaymentInvoceDto
    {
        /// <summary>
        /// Obtiene o Almacena el Tipo de Pago (0 Efectivo, 1 Tarjeta, 2 Cheque, 3 Deposito o Transferencia) /
        /// Gets or Sets the Payment Type (0 Cash, 1 Credit or Debit Card , 2 Check , 3 Deposit or Transference).
        /// </summary>
        /// <value>
        /// Tipo de Pago / Payment Type .
        /// </value>
        [Range(0, 4, ErrorMessage = "El tipo de pago debe estar entre 0 y 4")]
        [Required(ErrorMessage = "Se debe indicar el tipo de forma de pago")]
        public int TypePayment { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de comprobante o Deposito / Gets or Sets the Reference number of the Deposit or Transfer.
        /// </summary>
        /// <value>
        /// Número de comprobante o Deposito / Reference number of the Deposit or Transfer.
        /// </value>
        public string Trans { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto Pagado / Gets or Sets the Amount Paid.
        /// </summary>
        /// <value>
        /// Monto Pagado / Amount Paid.
        /// </value>
        [Required(ErrorMessage = "Se debe ingresar monto de la forma de pago")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Banco del deposito o transferencia (Puede ser null) / Gets or Sets the Bank Id (Can be null)
        /// </summary>
        public Guid? BankId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el usuario de la tarjeta / Gets or Sets the Client Card number.
        /// </summary>
        public string UserCard { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Banco / Gets or sets the name of the bank.
        /// </summary>
        /// <value>
        /// Nombre del Banco / The name of the bank.
        /// </value>
        public string BankName { get; set; }

        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene el ID del pago / Gets Payment Id.
        /// </summary>
        /// <value>
        /// Id del Pago / Payment Id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena la forma de Pago (0 Efectivo, 1 Tarjeta, 2 Cheque, 3 Deposito o Transferencia) /
        /// Gets or Sets the Payment method Type (0 Cash, 1 Credit or Debit Card , 2 Check , 3 Deposit or Transference).
        /// </summary>
        /// <value>
        /// Forma de Pago / Payment Method Type .
        /// </value>
        public PaymetnMethodType PaymetnMethodType { get; set; }

    }
}
