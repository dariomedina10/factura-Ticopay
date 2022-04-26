using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using TicoPay.Users;
using TicoPay.General;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using TicoPay.Invoices.XSD;
using System.Security.Cryptography.X509Certificates;
using System;
using TicoPay.Common;
using System.Web;
using TicoPay.Sellers;
using TicoPay.Sellers.Dto;

namespace TicoPay.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Tenant))]
    public class CreateTenantInput : IDtoViewBaseFields, ICustomValidate
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(Tenant.TenancyNameRegex)]
        [Display(Name = "Sub-Dominio")]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(Tenant.MaxNameLength)]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required]
        [StringLength(User.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [MaxLength(AbpTenantBase.MaxConnectionStringLength)]
        public string ConnectionString { get; set; }
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }
        public virtual Barrio Barrio { get; set; }
        public int? BarrioId { get; set; }
        public IList<Barrio> BarriosList { get; set; }
        public IList<Provincia> Province { get; set; }
        public int ProvinciaID { get; set; }
        public int CantonID { get; set; }
        public IList<Canton> Cantons { get; set; }
        public IList<Distrito> Distritos { get; set; }
        public int DistritoID { get; set; }

        [MaxLength(3)]
        public string local { get; set; }

        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
        public virtual Country Country { get; set; }
        public int? CountryID { get; set; }

        [MaxLength(80)]
        [Display(Name = "Nombre Comercial")]
        public string BussinesName { get; set; }


        public IdentificacionTypeTipo IdentificationType { get; set; }

        [MaxLength(12)]
        public string IdentificationNumber { get; set; }

        [MaxLength(80)]
        [Display(Name = "Nombre Comercial")]
        public string ComercialName { get; set; }

        [MaxLength(160)]
        public string Address { get; set; }

        [StringLength(23)]
        public string PhoneNumber { get; set; }

        [StringLength(23)]
        public string Fax { get; set; }

        [StringLength(60)]
        [EmailAddress]
        public string Email { get; set; }
        public int EditionId { get; set; }
        public string EditionDisplayName { get; set; }
        public string EditionName { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public IList<Country> CountrySelect { get; set; }

        [Required]
        [StringLength(User.MaxNameLength)]
        public string AdminName { get; set; }

        [Required]
        [StringLength(User.MaxSurnameLength)]
        public string AdminSurname { get; set; }

        [Required]
        [StringLength(User.MaxUserNameLength)]
        public string AdminUserName { get; set; }

        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 3 characters required")]
        [Required, StringLength(User.MaxPasswordLength,MinimumLength = 6)]
        public string AdminPassword { get; set; }

        public int StepRegister { get; set; }

        public virtual Seller Seller { get; set; }
        public int SellerCode { get; set; }
        public IList<Seller> Sellers { get; set; }

        public List<System.Web.Mvc.SelectListItem> IdentificationTypes
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.Cedula_Fisica.ToString(), Text = "Cédula Física" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.Cedula_Juridica.ToString(), Text = "Cédula Jurídica" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.DIMEX.ToString(), Text = "DIMEX" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.NITE.ToString(), Text = "NITE" });
                return list;
            }
        }

        public List<System.Web.Mvc.SelectListItem> ConditionSaleTypes
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Apartado.ToString(), Text = "Apartado" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Arrendamiento_Funcion_Financiera.ToString(), Text = "Arrendamiento Función Financiera" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Arrendamiento_Opcion_de_Compra.ToString(), Text = "Arrendamiento Opción de Compra" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Consignacion.ToString(), Text = "Consignación" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Contado.ToString(), Text = "Contado" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Credito.ToString(), Text = "Crédito" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaCondicionVenta.Otros.ToString(), Text = "Otros" });
                return list;
            }
        }

        public HttpPostedFileBase Logo { get; set; }
        public int InvoicesMonthlyLimitNotificationInterval { get; set; }

        public CreateTenantInput()
        {
            CountrySelect = new List<Country>();
            Distritos = new List<Distrito>();
            BarriosList = new List<Barrio>();
            Cantons = new List<Canton>();
            Sellers = new List<Seller>();
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            //if (ProvinciaID <= 0)
            //    context.Results.Add(new ValidationResult("Debe seleccionar una Provincia", new string[] { "ProvinciaID" }));

            //if (BarrioId <= 0)
            //    context.Results.Add(new ValidationResult("Debe seleccionar un Barrio", new string[] { "BarrioId" }));

            //if (DistritoID <= 0)
            //    context.Results.Add(new ValidationResult("Debe seleccionar un Distrito", new string[] { "DistritoID" }));

            //if (CantonID <= 0)
            //    context.Results.Add(new ValidationResult("Debe seleccionar un Canton", new string[] { "CantonID" }));

            // password

            if (AdminPassword.Length < 6)
                context.Results.Add(new ValidationResult("La contraseña debe minimo 6 dígitos", new string[] { "AdminPassword" }));


            // validacion de cedula fisica
            if ((IdentificationNumber.Length == 9) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
            {
                if (IdentificationNumber.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número de Cédula física, no debe tener un 0 al inicio", new string[] { "IdentificationNumber" }));
            }
            else if (((IdentificationNumber.Length > 9) || (IdentificationNumber.Length < 9)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                context.Results.Add(new ValidationResult("El número de Cédula física, debe contener 9 dígitos", new string[] { "IdentificationNumber" }));

            // validacion de cedula juridica
            if (((IdentificationNumber.Length > 10) || (IdentificationNumber.Length < 10)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Juridica))
                context.Results.Add(new ValidationResult("El número de cédula jurídica, debe contener 10 dígitos", new string[] { "IdentificationNumber" }));

            // validacion de DIMEX
            if (((IdentificationNumber.Length == 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((IdentificationNumber.Length == 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
            {
                if (IdentificationNumber.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número DIMEX, no debe tener un 0 al inicio", new string[] { "IdentificationNumber" }));
            }
            else if (((IdentificationNumber.Length < 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((IdentificationNumber.Length > 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
                context.Results.Add(new ValidationResult("El número DIMEX, debe contener 11 0 12 dígitos", new string[] { "IdentificationNumber" }));

            // Validacion NITE
            if (((IdentificationNumber.Length < 10) || (IdentificationNumber.Length > 10)) && (IdentificationType == IdentificacionTypeTipo.NITE))
                context.Results.Add(new ValidationResult("El número NITE, debe contener 10 dígitos", new string[] { "IdentificationNumber" }));
        }
    }
}