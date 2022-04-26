using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TicoPay.Application.Helpers;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Drawers;
using TicoPay.General;
using TicoPay.Inventory;
using TicoPay.Invoices.XSD;
using TicoPay.Services;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    public class CreateInvoiceInput : IDtoViewBaseFields, IUnityInvoice, ICustomValidate
    {
        public Guid? ClientId { get; set; }
        public virtual IList<ItemInvoice> InvoiceLines { get; set; }
        public PaymetnMethodType PaymentType { get; set; }
        public string Transaction { get; set; }

        public decimal? DiscountGeneral { get; set; }
        public int? TypeDiscountGeneral { get; set; }

        public int? DayCredit { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        
        public IList<Service> ListService { get; set; }

        public int? PaymentTypeSelect { get; set; }
        public IEnumerable<SelectListItem> PaymentTypes { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public int ConditionSaleType { get; set; }

        public IEnumerable<SelectListItem> ConditionSaleTypes { get; set; }
        public int BankId { get; set; }
        public IEnumerable<SelectListItem> PaymentInvoiceBanks { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda Coin { get; set; }

        public IEnumerable<SelectListItem> CoinType { 

            get
            {
                // var list = EnumHelper.GetSelectListValues(typeof(FacturaElectronicaResumenFacturaCodigoMoneda));

                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString(), Text = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString() });
                list.Add(new SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString(), Text = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString() });
                return list;
            }
        }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        public IEnumerable<SelectListItem> FirmTypes { get; set; }

        public bool ValidateHacienda { get; set; }

        public TypeDocumentInvoice TypeDocument { get; set; } = TypeDocumentInvoice.Invoice;

        public bool IsInvoice { get; set; }

        public string ClientName { get; set; }

        public IdentificacionTypeTipo? ClientIdentificationType { get; set; }
        /// <summary>
        /// Gets or sets the identification of your client. 
        /// </summary>

        [StringLength(20)]
        public string ClientIdentification { get; set; }

        public string ClientAddress { get; set; }

     
        public string ClientPhoneNumber { get; set; }
               
        public string ClientMobilNumber { get; set; }

        
        public string ClientEmail { get; set; }


        public bool IsContingency { get; set; } = false;

        public string ConsecutiveNumberContingency { get; set; }

        public string ReasonContingency { get; set; }

        public DateTime? DateContingency { get; set; }

        public Drawer Drawer { get; set; }

        public string GeneralObservation { get; set; }

        public int IsPos { get; set; }
       
        public PrinterTypes? PrinterType { get; set; }

        public IList<System.Web.Mvc.SelectListItem> IdentificationTypes
        {
            get
            {
                var list = EnumHelper.GetSelectList(typeof(IdentificacionTypeTipo));
                return list;
            }
        }

        public void AddValidationErrors(CustomValidationContext context)
        {

            //if ((!String.IsNullOrWhiteSpace(ClientEmail)) && (!Regex.IsMatch(ClientEmail,
            //  @"[a-zA-Z][\w+.-]+@[\w+.-]+\.[a-zA-Z0-9]{2,5}$")))
            //    context.Results.Add(new ValidationResult("Dirección de correo inválida.", new string[] { "ClientEmail" }));

            if ((!String.IsNullOrWhiteSpace(ClientPhoneNumber)) && (!Regex.IsMatch(ClientPhoneNumber,
             @"^[0-9]{3}-[0-9]{8}$")))
                context.Results.Add(new ValidationResult("Formato de número telefónico inválido. Ej: 506-99999999", new string[] { "ClientPhoneNumber" }));

            if ((!String.IsNullOrWhiteSpace(ClientMobilNumber)) && (!Regex.IsMatch(ClientMobilNumber,
           @"^\d{3}-\d{8}$")))
                context.Results.Add(new ValidationResult("Formato de número móvil inválido. Ej: 506-99999999", new string[] { "ClientMobilNumber" }));

            if ((ClientIdentificationType!=null) &&(ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda))
            {
                if ((ClientIdentification.Length == 9) && (ClientIdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                {
                    if (ClientIdentification.IndexOf("0") == 0)
                        context.Results.Add(new ValidationResult("El número de Cédula física, no debe tener un 0 al inicio", new string[] { "Identification" }));
                }
                else
                if (((ClientIdentification.Length > 9) || (ClientIdentification.Length < 9)) && (ClientIdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                    context.Results.Add(new ValidationResult("El número de Cédula física, debe contener 9 dígitos", new string[] { "Identification" }));

                // validacion de cedula juridica

                if (((ClientIdentification.Length > 10) || (ClientIdentification.Length < 10)) && (ClientIdentificationType == IdentificacionTypeTipo.Cedula_Juridica))
                    context.Results.Add(new ValidationResult("El número de cédula jurídica, debe contener 10 dígitos", new string[] { "Identification" }));

                // validacion de DIMEX

                if (((ClientIdentification.Length == 11) && (ClientIdentificationType == IdentificacionTypeTipo.DIMEX)) || ((ClientIdentification.Length == 12) && (ClientIdentificationType == IdentificacionTypeTipo.DIMEX)))
                {
                    if (ClientIdentification.IndexOf("0") == 0)
                        context.Results.Add(new ValidationResult("El número DIMEX, no debe tener un 0 al inicio", new string[] { "Identification" }));
                }
                else
                    if (((ClientIdentification.Length < 11) && (ClientIdentificationType == IdentificacionTypeTipo.DIMEX)) || ((ClientIdentification.Length > 12) && (ClientIdentificationType == IdentificacionTypeTipo.DIMEX)))
                    context.Results.Add(new ValidationResult("El número DIMEX, debe contener 11 0 12 dígitos", new string[] { "Identification" }));

                // validacion de NITE

                if (((ClientIdentification.Length < 10) || (ClientIdentification.Length > 10)) && (ClientIdentificationType == IdentificacionTypeTipo.NITE))
                    context.Results.Add(new ValidationResult("El número NITE, debe contener 10 dígitos", new string[] { "Identification" }));
            }




        }
    }

    /// <summary>
    /// Clase que almacena los datos de las lineas de la factura / Contains the information of the Invoice Lines
    /// </summary>
    public class ItemInvoice
    {
        /// <summary>
        /// Obtiene o Almacena el Id de la Linea de la factura / Gets or Sets the Invoice Line Id.
        /// </summary>
        /// <value>
        /// Id de la Linea / Invoice Line Id.
        /// </value>
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Servicio a Facturar (Si esta Creado en Ticopay, Sino enviar en Null) / 
        /// Gets or Sets the Service Id (If you created the service in Ticopays , otherwise send it null).
        /// </summary>
        /// <value>
        /// Id del Servicio a Facturar / Service Id.
        /// </value>
        public Guid IdService { get; set; }
        
        /// Obtiene o Almacena el Id del Producto a Facturar (Si esta Creado en Ticopay, Sino enviar en Null) /
        /// Gets or Sets the Product Id (If you created the Product in Ticopays, otherwise send it null).
        /// </summary>
        /// <value>
        /// Id del Producto a Facturar / Product Id.
        /// </value>
        [JsonIgnore]
        public Guid IdProducto { get; set; }

        /// Obtiene o Almacena el Id del Producto a Facturar (Si esta Creado en Ticopay, Sino enviar en Null) /
        /// Gets or Sets the Product Id (If you created the Product in Ticopays, otherwise send it null).
        /// </summary>
        /// <value>
        /// Id del Producto a Facturar / Product Id.
        /// </value>
        public Guid IdProduct { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Cantidad a Facturar / Gets or Sets the Quantity of the Item.
        /// </summary>
        /// <value>
        /// Cantidad a Facturar / Quantity.
        /// </value>
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Porcentaje del Descuento por Linea. / Gets or Sets the Discount Percentage of the item
        /// </summary>
        /// <value>
        /// Porcentaje del Descuento por Linea / Discount Percentage of the Line.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Descuento { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Impuesto a Aplicar a la Linea / Gets or Sets the Tax Id of the item.
        /// </summary>
        /// <value>
        /// Id del Impuesto a Aplicar / Tax Id.
        /// </value>
        [Required]
        public Guid IdImpuesto { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Impuesto de la Linea / Gets or Sets the Tax Amount.
        /// </summary>
        /// <value>
        /// Monto del Impuesto de la Linea / Tax Amount.
        /// </value>
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Impuesto { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio de la Linea / Gets or Sets the Item Price.
        /// </summary>
        /// <value>
        /// Precio de la Linea / Price.
        /// </value>
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Precio { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Linea / Gets or Sets the Item Description.
        /// </summary>
        /// <value>
        /// Descripción de la Linea / Item Description.
        /// </value>
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string Servicio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea / Gets or Sets the Line Total.
        /// </summary>
        /// <value>
        /// Total de la Linea / Line Total.
        /// </value>
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtiene el Total del Descuento de la Linea / Gets or Sets the Line Total Discount.
        /// </summary>
        /// <value>
        /// Total del Descuento de la Linea / Line Total Discount.
        /// </value>
        public decimal TotalDescuento { get; set; }

        /// <summary>
        /// Obtiene el Total del Impuesto de la Linea / Gets or Sets the Line Tax Total.
        /// </summary>
        /// <value>
        /// Total del Impuesto de la Linea / Line Tax Total.
        /// </value>
        public decimal TotalImpuesto { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida de la Linea / Gets or Sets the Line Measurement Unit.
        /// </summary>
        /// <value>
        /// Unidad de Medida de la Linea / Line Measurement Unit.
        /// </value>
        public UnidadMedidaType? UnidadMedida { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de Medida en caso de ser Otra / Gets or Sets the Description of the Other Line Measurement Unit.
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de Medida / Description of the Other Line Measurement Unit.
        /// </value>
        public string UnidadMedidaOtra { get; set; }

        /// <summary>
        /// Obtiene o Almacena una Nota adicional sobre la Linea / Gets or Sets a Note about the Line.
        /// </summary>
        /// <value>
        /// Nota adicional sobre la Linea / Line Note.
        /// </value>
        public string Note { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Linea / Gets or sets the Line Type.
        /// </summary>
        /// <value>
        /// Tipo de Linea / Line Type.
        /// </value>
        public LineType Tipo { get; set; } = LineType.Service;

        /// <summary>
        /// Obtiene o Almacena una Descripción del descuento sobre la Linea / Gets or Sets the Discount Description.
        /// </summary>
        /// <value>
        /// Nota adicional sobre Descuento de la Linea / Line Discount Description.
        /// </value>
        public string DescriptionDiscount { get; set; }
        
    }

    
}
