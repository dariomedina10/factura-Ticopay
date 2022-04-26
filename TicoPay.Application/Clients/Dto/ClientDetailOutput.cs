using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay.Clients.Dto
{
    [AutoMapFrom(typeof(Client))]
    public class ClientDetailOutput: IDtoViewBaseFields
    {
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        public IdentificacionTypeTipo IdentificationType { get; set; }

        [Display(Name = "Identificación")]
        public string Identification { get; set; }
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Código")]
        public long Code { get; set; }

        public Guid? GroupId { get; protected set; }
        [Display(Name = "Género")]
        public Gender Gender { get; set; }
        [Display(Name = "Nro. Teléfono")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Nro. Celular")]
        public string MobilNumber { get; protected set; }
        [Display(Name = "Nro. Fax")]
        public string Fax { get; set; }
        [Display(Name = "Correo Electrónico")]
        public string Email { get; protected set; }

        public string WebSite { get; protected set; }

        public string PostalCode { get; set; }

        public string IsoCountry { get; set; }

        public string Address { get; set; }

        public long? Latitud { get; set; }

        public long? Longitud { get; set; }

        public string ContactName { get; set; }

        public string ContactMobilNumber { get; set; }

        public string ContactPhoneNumber { get; set; }

        public string ContactEmail { get; set; }

        public string Note { get; set; }

        public decimal Balance { get; protected set; }

        public string ImageUrl { get; protected set; }

        public bool AllowEmailNotifications { get; protected set; }

        public bool AllowSmsNotifications { get; protected set; }

        public State State { get; protected set; }

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<ClientService> Services { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }

        public int CreatorUserId { get; set; }

        public IList<User> Users { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
