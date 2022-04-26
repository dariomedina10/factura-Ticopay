using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Services;
using TicoPay.General;
using Abp.Runtime.Validation;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using TicoPay.GroupConcept.Dto;
using TicoPay.Application.Helpers;
using TicoPay.Web.DataAnnotation;
using System.ComponentModel;

namespace TicoPay.Clients.Dto
{
    [AutoMapFrom(typeof(Client))]
    public class UpdateClientInput : IDtoViewBaseFields, ICustomValidate
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(Client.MaxNameLength)]
        public string Name { get; set; }

        //[Required]
        [StringLength(Client.MaxNameLength)]
        public string LastName { get; set; }

        [StringLength(Client.MaxNameLength)]
        public string NameComercial { get; set; }
        public IdentificacionTypeTipo IdentificationType { get; set; }
        public long Code { get; set; }

        
        [StringLength(Client.MaxIdentificationLength)]
        public string Identification { get; set; }
        public string IdentificacionExtranjero { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        public Guid? GroupId { get; set; }

        public Gender Gender { get; set; }

        [StringLength(Client.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [StringLength(Client.MaxPhoneNumberLength)]
        public string MobilNumber { get; set; }

        [StringLength(Client.MaxPhoneNumberLength)]
        public string Fax { get; set; }

        [StringLength(Client.MaxEmailLength)]
        [EmailAttribute]
        [DisplayName("Correo Electrónico")]
        public string Email { get; set; }

        [StringLength(Client.MaxUrlLength)]
        public string WebSite { get; set; }

        public virtual Barrio Barrio { get; set; }
        public int? BarrioId { get; set; }
        public IList<Barrio> BarriosList { get; set; }
        public IList<Provincia> Province { get; set; }
        public int ProvinciaID { get; set; }
        public int CantonID { get; set; }
        public IList<Canton> Cantons { get; set; }
        public IList<Distrito> Distritos { get; set; }
        public IList<Country> Paises { get; set; }
        public int? CountryID { get; set; }
        public int DistritoID { get; set; }

        [StringLength(Client.MaxPostalCodeLength)]
        public string PostalCode { get; set; }

        [StringLength(Client.MaxIsoCountryLength)]
        public string IsoCountry { get; set; }

        [StringLength(Client.MaxAddressLength)]
        public string Address { get; set; }

        [StringLength(Client.MaxNameLength)]
        public string ContactName { get; set; }

        [StringLength(Client.MaxPhoneNumberLength)]
        public string ContactMobilNumber { get; set; }

        [StringLength(Client.MaxPhoneNumberLength)]
        public string ContactPhoneNumber { get; set; }


        [StringLength(180, ErrorMessage = "La suma de los caracteres de los tres Correo del contacto: no puedes ser mayor a 180")]
        [DisplayName("Correo del contacto")]
        public string ContactEmail { get; set; }

        [StringLength(Client.MaxNoteLength)]
        public string Note { get; set; }

        public long? Latitud { get; set; }

        public long? Longitud { get; set; }

        /// <summary>
        /// Gets or sets the Groups List
        /// </summary>
        public virtual ICollection<ClientGroup> Groups { get;  set; }

        //public virtual ICollection<Service> Services { get;  set; }

        public IList<Group> GroupsSelect { get; set; }

        public string[] GroupsSelected { get; set; }

        public IList<GroupConcepts> GroupsConceptsSelect { get; set; }

        public IList<GroupConceptsDto> GroupsConceptsList { get; set; }

        public IList<ServiceDto> ClientServiceList { get; set; }

        public IList<ClientService> ClientServices { get; set; }

        public IList<Service> ServicesSelect { get; set; }

        public IList<Tipos> TypesSelect { get; set; }

        public string[] TypesList { get; set; }

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }
        
        
        public int? CreditDays { get; set; }

        public IList<System.Web.Mvc.SelectListItem> IdentificationTypes

        {
            get
            {
                var list = EnumHelper.GetSelectList(typeof(IdentificacionTypeTipo));
                return list;
            }
        }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public bool CanEditCostoSms { get; set; }
        public decimal CostoSms { get; set; }

        public UpdateClientInput()
        {
            GroupsSelect=new List<Group>();
            GroupsConceptsSelect = new List<GroupConcepts>();
        }


        public void AddValidationErrors(CustomValidationContext context)
        {
            // si selecciona provincia, debe completar todos los datos de direccion
            if ((ProvinciaID > 0) || (BarrioId > 0) || (DistritoID > 0) || (CantonID > 0) || (!string.IsNullOrEmpty(Address)))
            {
                if (ProvinciaID <= 0)
                    context.Results.Add(new ValidationResult("Debe seleccionar una Provincia", new string[] { "ProvinciaID" }));

                if (BarrioId.GetValueOrDefault() <= 0)
                    context.Results.Add(new ValidationResult("Debe seleccionar un Barrio", new string[] { "BarrioId" }));

                if (DistritoID <= 0)
                    context.Results.Add(new ValidationResult("Debe seleccionar un Distrito", new string[] { "DistritoID" }));

                if (CantonID <= 0)
                    context.Results.Add(new ValidationResult("Debe seleccionar un Canton", new string[] { "CantonID" }));

                if (string.IsNullOrEmpty(Address))
                    context.Results.Add(new ValidationResult("Debe ingresar Otras señas", new string[] { "Address" }));
            }

            // validacion de cedula fisica
            if (IdentificationType != IdentificacionTypeTipo.NoAsiganda) {
                if ((Identification.Length == 9) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                {
                    if (Identification.IndexOf("0") == 0)
                        context.Results.Add(new ValidationResult("El número de Cédula física, no debe tener un 0 al inicio", new string[] { "Identification" }));
                }
                else
             if (((Identification.Length > 9) || (Identification.Length < 9)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                    context.Results.Add(new ValidationResult("El número de Cédula física, debe contener 9 dígitos", new string[] { "Identification" }));

                // validacion de cedula juridica

                if (((Identification.Length > 10) || (Identification.Length < 10)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Juridica))
                    context.Results.Add(new ValidationResult("El número de cédula jurídica, debe contener 10 dígitos", new string[] { "Identification" }));

                // validacion de DIMEX

                if (((Identification.Length == 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((Identification.Length == 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
                {
                    if (Identification.IndexOf("0") == 0)
                        context.Results.Add(new ValidationResult("El número DIMEX, no debe tener un 0 al inicio", new string[] { "Identification" }));
                }
                else
                    if (((Identification.Length < 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((Identification.Length > 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
                    context.Results.Add(new ValidationResult("El número DIMEX, debe contener 11 0 12 dígitos", new string[] { "Identification" }));
                  
                // validacion de NITE

                if (((Identification.Length < 10) || (Identification.Length > 10)) && (IdentificationType == IdentificacionTypeTipo.NITE))
                    context.Results.Add(new ValidationResult("El número NITE, debe contener 10 dígitos", new string[] { "Identification" }));
            }

             
        }
    }
}
