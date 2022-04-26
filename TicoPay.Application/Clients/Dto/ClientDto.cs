using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;

namespace TicoPay.Clients.Dto
{
    [AutoMapFrom(typeof(Client))]
    public class ClientDto : EntityDto<Guid>, ICustomValidate
    {
        /// <summary>
        /// Obtiene o Almacena el nombre del Cliente / Gets or Sets the Client Name.
        /// </summary>
        /// <value>
        /// Nombre del Cliente / Client Name.
        /// </value>
        [Required]
        [StringLength(Client.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Apellido del Cliente / Gets or Sets the Client Last Name.
        /// </summary>
        /// <value>
        /// Apellido del Cliente / Client Last Name.
        /// </value>
        [StringLength(Client.MaxNameLength)]
        public string LastName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Identificación del Cliente / Gets of Sets the Client Identification Type.
        /// </summary>
        /// <value>
        /// Tipo de Identificación del Cliente / Client Identification Type.
        /// </value>        
        [Range(0, 4, ErrorMessage = "El tipo de identificación debe estar entre 0 y 4")]
        [Required(ErrorMessage = "Se debe indicar el tipo de identificación")]
        public IdentificacionTypeTipo IdentificationType { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Identificación del Cliente / Gets or Sets the Client Identification number.
        /// </summary>
        /// <value>
        /// Número de Identificación del Cliente / Client Identification number.
        /// </value>
        [Required(ErrorMessage = "Se debe indicar el número de identificación del cliente")]
        [StringLength(Client.MaxIdentificationLength)]
        public string Identification { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Identificación del Cliente Extranjero (Nº Pasaporte) / Gets or Sets the Client Foreign Identification number.
        /// </summary>
        /// <value>
        /// Número de Identificación del Cliente Extranjero / Client Foreign Identification number.
        /// </value>
        public string IdentificacionExtranjero { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre Comercial del Cliente / Gets or Sets the Client Business name.
        /// </summary>
        /// <value>
        /// Nombre Comercial del Cliente / Client Business name.
        /// </value>
        [StringLength(Client.MaxNameLength)]
        public string NameComercial { get; set; }

        [JsonIgnore]
        public int TipoId { get; set; }

        /// <summary>
        /// Obtiene el código interno de cliente
        /// </summary>
        /// <value>
        /// código interno de cliente.
        /// </value>
        [JsonIgnore]
        public long Code { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Id del Grupo (Categoría) al cual pertenece el cliente.
        /// </summary>
        /// <value>
        /// Id del Grupo.
        /// </value>
        [JsonIgnore]
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de Cumpleaños del Cliente / Gets or Sets the Client Birthday date.
        /// </summary>
        /// <value>
        /// Fecha de Cumpleaños del Cliente / Client Birthday date.
        /// </value>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Genero del Cliente / Gets or Sets the Clients Gender
        /// </summary>
        /// <value>
        /// Genero del Cliente / Clients Gender.
        /// </value>
        public Gender Gender { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número Telefónico del Cliente / Gets or Sets the Client Phone Number.
        /// </summary>
        /// <value>
        /// Número Telefónico del Cliente / Clients Phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Teléfono Celular del Cliente / Gets or Sets the Client Cellphone number.
        /// </summary>
        /// <value>
        /// Número de Teléfono Celular / Client Cellphone number.
        /// </value>
        public string MobilNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Fax del Cliente / Gets or Sets the Fax Phone number.
        /// </summary>
        /// <value>
        /// Número de Fax / Fax Phone number.
        /// </value>
        public string Fax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Correo Electrónico del Cliente / Gets or Sets the Client Email.
        /// </summary>
        /// <value>
        /// Correo Electrónico / Email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o Almacena la dirección Web del cliente / Gets or Sets the Client Web Address.
        /// </summary>
        /// <value>
        /// Dirección Web del cliente / Client Web Address.
        /// </value>
        public string WebSite { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Barrio del cliente / Gets or Sets the Client Hood Id.
        /// </summary>
        /// <value>
        /// Id del Barrio / Hood Id.
        /// </value>
        public int BarrioId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del País / Gets or sets the country Id.
        /// </summary>
        /// <value>
        /// Id del País / Country Id.
        /// </value>
        public int? CountryId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código Postal del Cliente / Gets or Sets the Client Postal Code.
        /// </summary>
        /// <value>
        /// Código Postal / Postal Code.
        /// </value>
        public string PostalCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código ISO del País del Cliente .
        /// </summary>
        /// <value>
        /// Código ISO del País.
        /// </value>
        [JsonIgnore]
        public string IsoCountry { get; set; }

        /// <summary>
        /// Obtiene o Almacena la dirección del Cliente / Gets the Client Address
        /// </summary>
        /// <value>
        /// Dirección del Cliente.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Latitud (Posicionamiento) del Cliente
        /// </summary>
        /// <value>
        /// Latitud (Posicionamiento).
        /// </value>
        [JsonIgnore]
        public long Latitud { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Longitud (Posicionamiento) del Cliente
        /// </summary>
        /// <value>
        /// Longitud (Posicionamiento).
        /// </value>
        [JsonIgnore]
        public long Longitud { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre de Contacto del Personal del cliente / Gets or Sets the Client Contact Name.
        /// </summary>
        /// <value>
        /// Nombre de Contacto del Personal / Contact Name.
        /// </value>
        public string ContactName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de teléfono Móvil del contacto de la empresa / Gets or Sets the Client Contact Cellphone number.
        /// </summary>
        /// <value>
        /// Número de teléfono Móvil del contacto / Client Contact Cellphone number.
        /// </value>
        public string ContactMobilNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de teléfono del contacto de la empresa / Gets or Sets the Client Contact Phone number.
        /// </summary>
        /// <value>
        /// Número de teléfono del contacto / Client Contact Phone number.
        /// </value>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Dirección de Correo Electrónico del Contacto de la empresa / Gets or Sets the Client Contact Email.
        /// </summary>
        /// <value>
        /// Dirección de Correo Electrónico del Contacto / Client Contact Email.
        /// </value>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the balance of your client
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        [JsonIgnore]
        public decimal Balance { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Categorías a las que Pertenece el Cliente / Gets the Categories to which the Client belongs.
        /// </summary>
        /// <value>
        /// Categorías a las que Pertenece el Cliente / Categories to which the Client belongs.
        /// </value>
        public List<Group> Categories { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Imagen o Logo del Cliente.
        /// </summary>
        /// <value>
        /// Imagen o Logo del Cliente.
        /// </value>
        [JsonIgnore]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Obtiene o Almacena un Nota adicional sobre el Cliente / Gets or Sets a Note about the Client.
        /// </summary>
        /// <value>
        /// Nota adicional sobre el Cliente / Note about the Client.
        /// </value>
        public string Note { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado donde se encuentra el Cliente / Gets or Sets the Client status
        /// </summary>
        /// <value>
        /// Estado donde se encuentra el Cliente / Client Status.
        /// </value>
        public State State { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Cantidad de Días de Crédito del Cliente / Gets or Sets the Client Credit days
        /// </summary>
        /// <value>
        /// Cantidad de Días de Crédito / Credit Days.
        /// </value>
        public int? CreditDays { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool CanBeDelete { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDto"/> class.
        /// </summary>
         public ClientDto()
        {
            //Invoices = new List<InvoiceDto>();
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            // validacion de Email

            //if (Email == null || Email == "")
            //{
            //    context.Results.Add(new ValidationResult("Debe proporcionar un Email Valido", new string[] { "Email" }));
            //}

            // validacion de apellido

            if ((IdentificationType != IdentificacionTypeTipo.Cedula_Juridica) && (LastName == null || LastName.Trim() == string.Empty))
            {
                context.Results.Add(new ValidationResult("Para el tipo de identificación seleccionada el Apellido es obligatorio", new string[] { "LastName" }));
            }


            // validacion de cedula fisica

            if ((IdentificationType == IdentificacionTypeTipo.Cedula_Fisica) && (Identification.Length == 9))
            {
                if (Identification.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número de Cédula física, no debe tener un 0 al inicio", new string[] { "Identification" }));                
            }
            else
                if ((IdentificationType == IdentificacionTypeTipo.Cedula_Fisica) &&((Identification.Length > 9) || (Identification.Length < 9)) )
                context.Results.Add(new ValidationResult("El número de Cédula física, debe contener 9 dígitos", new string[] { "Identification" }));

            // validacion de cedula juridica

            if ((IdentificationType == IdentificacionTypeTipo.Cedula_Juridica) && ((Identification.Length > 10) || (Identification.Length < 10)))
                context.Results.Add(new ValidationResult("El número de cédula jurídica, debe contener 10 dígitos", new string[] { "Identification" }));


            // validacion de DIMEX

            if (((IdentificationType == IdentificacionTypeTipo.DIMEX) && (Identification.Length == 11)) || ((IdentificationType == IdentificacionTypeTipo.DIMEX) && (Identification.Length == 12)))
            {
                if (Identification.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número DIMEX, no debe tener un 0 al inicio", new string[] { "Identification" }));
            }
            else
                if (((IdentificationType == IdentificacionTypeTipo.DIMEX) && (Identification.Length < 11)) || ((IdentificationType == IdentificacionTypeTipo.DIMEX) && (Identification.Length > 12)))
                context.Results.Add(new ValidationResult("El número DIMEX, debe contener 11 0 12 dígitos", new string[] { "Identification" }));

            // validacion de NITE

            if ((IdentificationType == IdentificacionTypeTipo.NITE) && ((Identification.Length < 10) || (Identification.Length > 10)))
                context.Results.Add(new ValidationResult("El número NITE, debe contener 10 dígitos", new string[] { "Identification" }));

            // Validacion Extranjero

            if (IdentificationType == IdentificacionTypeTipo.NoAsiganda)
            {
                if((IdentificacionExtranjero == null) || (IdentificacionExtranjero.Length < 5))
                {
                    context.Results.Add(new ValidationResult("El número identificación de extranjeros, No puede estar vacío y debe contener mínimo 5 dígitos", new string[] { "IdentificationExtranjero" }));
                }
            }
                


        }
    }
}
