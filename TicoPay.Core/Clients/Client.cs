using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Events.Bus;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TicoPay.General;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using System.Xml.Serialization;
using TicoPay.Invoices.XSD;
using Abp.Dependency;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TicoPay.Clients
{
    public class Client : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public const int MaxNameLength = 200;
        public const int MaxIdentificationLength = 12;
        public const int MaxIdentificationExtranjeroLength = 20;
        public const int MaxNoteLength = 2048;
        public const int MaxPhoneNumberLength = 23;
        public const int MaxEmailLength = 60;
        public const int MaxUrlLength = 1000;
        public const int MaxAddressLength = 160;
        public const int MaxStreetLength = 150;
        public const int MaxCityLength = 150;
        public const int MaxRegionLength = 150;
        public const int MaxPostalCodeLength = 15;
        public const int MaxIsoCountryLength = 3;

        /// <summary>Gets or sets the Name of your client. </summary>
        [Required]
        [StringLength(MaxNameLength)]
        [Display(Name = "Cliente")]
        public string Name { get; set; }

        [StringLength(MaxNameLength)]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [StringLength(MaxNameLength)]
        public string NameComercial { get; set; }

        /// <summary>
        /// Gets or sets the identification of your client. 
        /// </summary>
        public IdentificacionTypeTipo IdentificationType { get; set; }
        /// <summary>
        /// Gets or sets the identification of your client. 
        /// </summary>
        
        [StringLength(MaxIdentificationLength)]
        public string Identification { get; set; }

        [StringLength(MaxIdentificationExtranjeroLength)]
        public string IdentificacionExtranjero { get; set; }

        /// <summary>
        /// Gets or sets the Birthday of your client. 
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Gets or sets the Code of your client
        /// </summary>
       [Index("IX_Code", IsUnique = true)]
        [Display(Name = "Código")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the Gender of your client
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>Gets or sets the home phone of your client. </summary>
        [StringLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets the mobil number of your client. </summary>
        [StringLength(MaxPhoneNumberLength)]
        public string MobilNumber { get; set; }

        /// <summary>Gets or sets the fax number of your client. </summary>
        [StringLength(MaxPhoneNumberLength)]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the email of your client.
        /// </summary>
        [StringLength(MaxEmailLength)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the web site of your client.
        /// </summary>
        [StringLength(MaxUrlLength)]
        public string WebSite { get; set; }

        /// <summary>
        ///  Gets or sets the BarrioID in which you or your client is located.
        /// </summary>
        public virtual Barrio Barrio { get; set; }
        public int? BarrioId { get; set; }

        public virtual Country Country { get; set; }
        public int? CountryId { get; set; }
        /// <summary>
        /// Gets or sets the postal code in which you or your client is located.
        /// </summary>
        [StringLength(MaxPostalCodeLength)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the ISO country code of your or your client's address.
        /// </summary>
        [StringLength(MaxIsoCountryLength)]
        public string IsoCountry { get; set; }

        /// <summary>
        ///  Gets or sets the address where your client is located
        /// </summary>
        [StringLength(MaxAddressLength)]
        public string Address { get; set; }

        /// <summary>
        ///  Gets or sets the latitud where your client is located
        /// </summary>
        public long? Latitud { get; set; }

        /// <summary>
        ///  Gets or sets the Longitud where your client is located
        /// </summary>
        public long? Longitud { get; set; }

        /// <summary>Gets or sets the Name. </summary>
        [StringLength(MaxNameLength)]
        public string ContactName { get; set; }

        /// <summary>Gets or sets the mobil number of your client's contact. </summary>
        [StringLength(MaxPhoneNumberLength)]
        public string ContactMobilNumber { get; set; }

        /// <summary>Gets or sets the phone number of your client's contact. </summary>
        [StringLength(MaxPhoneNumberLength)]
        public string ContactPhoneNumber { get; set; }

        /// <summary>Gets or sets the email of your client's contact. </summary>
        [StringLength(MaxEmailLength)]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the balance of your client
        /// </summary>
        [Range(Double.MinValue, Double.MaxValue)]
        public decimal Balance { get; protected set; }

        /// <summary>
        /// Gets or sets the flag that indicates if it a client has a image associated
        /// </summary>
        public bool HasImage { get; protected set; }

        /// <summary>
        /// Gets or sets the image url of the client
        /// </summary>
        [StringLength(MaxUrlLength)]
        public string ImageUrl { get; protected set; }

        /// <summary>
        /// Gets or sets a Note. 
        /// </summary>
        [StringLength(MaxNoteLength)]
        public string Note { get; set; }

        /// <summary>Use this filed to indicates if your client allows email notifications. </summary>
        public bool AllowEmailNotifications { get; protected set; }

        /// <summary>Use this filed to indicates if your client allows sms notifications. </summary>
        public bool AllowSmsNotifications { get; protected set; }

        /// <summary>
        /// Gets or sets the State of your client
        /// </summary>
        public State State { get; protected set; }
        /// <summary>
        /// Tiene pago automatico banco
        /// </summary>
        public bool? PagoAutomaticoBn { get; set; }
        /// <summary>
        /// Dia en que se realiza el pago del servicio
        /// </summary>
        public int? DiaPagoBn { get; set; }
        /// <summary>
        /// Monto maximo autorizado
        /// </summary>
        public int? MontoMaximoBn { get; set; }
        /// <summary>
        /// forma de pago escogida
        /// </summary>
        public FormaPago? FormaPagoBn { get; set; }
        /// <summary>
        /// Gets or sets the Invoice List
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; protected set; }

        /// <summary>
        /// Gets or sets the Service List
        /// </summary>
        public virtual ICollection<ClientService> Services { get; protected set; }
        /// <summary>
        /// Gets or sets the Groups List
        /// </summary>
        public virtual ICollection<ClientGroup> Groups { get; protected set; }

        public virtual ICollection<ClientGroupConcept> GroupConcepts { get; protected set; }

        /// <summary>
        /// Cantidad de días que tiene el cliente
        /// </summary>
        public int? CreditDays { get; set; }

        //public virtual ICollection<ClientGroupConcept> ClientGroupConcepts { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public int TenantId { get; set; }

        public IEventBus EventBus { get; set; }

        /// <summary>
        /// We don't make constructor public and forcing to create clients using <see cref="Create"/> method.
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        public Client()
        {

        }

        public Client(int code, string name, string lastName, IdentificacionTypeTipo identificacionType, string identification, string identificacionExtranjero, string email,
            string mobilNumber)
        {
            var client = new Client
            {
                Code = code,
                Name = name,
                LastName = lastName,
                IdentificationType = identificacionType,
                Identification = identification,
                IdentificacionExtranjero = identificacionExtranjero,
                Email = email,
                MobilNumber = mobilNumber
            };
        }

        public static Client Create(int tenantId, string name, string lastName, Gender gender, DateTime? birthDate, string identification,
            IdentificacionTypeTipo identificationType, string phoneNumber, string mobilNumber,
            string fax, string email, string website, int? barrio,
            string postalCode, string isoCountry, string address, long? latitud, long? longitud, string contactName,
            string contactMobilNumber, string contactPhoneNumber, string contactEmail, string note, string namecomercial, string identifacionextranjero, int? creditDays, int? countryId)
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                LastName = lastName,
                NameComercial = namecomercial,// agregado para facturacion electronica
                Identification = identification,
                IdentificationType = identificationType,
                Birthday = birthDate,
                Gender = gender,
                PhoneNumber = phoneNumber,
                MobilNumber = mobilNumber,
                Fax = fax,
                Email = email,
                WebSite = website,
                PostalCode = postalCode,
                IsoCountry = isoCountry,
                Address = address,
                Latitud = latitud,
                Longitud = longitud,
                ContactName = contactName,
                ContactMobilNumber = contactMobilNumber,
                ContactPhoneNumber = contactPhoneNumber,
                ContactEmail = contactEmail,
                Note = note,
                State = State.Active,
                AllowEmailNotifications = true,
                AllowSmsNotifications = true,
                IdentificacionExtranjero = identifacionextranjero,
                CreditDays = creditDays,
                CountryId = countryId
            };
            client.Services = new Collection<ClientService>();
            client.Invoices = new Collection<Invoice>();
            client.GroupConcepts = new Collection<ClientGroupConcept>();
            if (barrio > 0)
                client.BarrioId = barrio;
            return client;
        }

        internal void AssignService(Service service, string cronExpresion, DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage)
        {
            AssertNotInactive();
            
            var serviceClient = ClientService.Create(service, this, cronExpresion, initDateTime, generateInvoice, allowLatePayment,  quantity,  discountPercentage);
            Services.Add(serviceClient);
        }

        internal void AssignService(Service service, string cronExpresion, DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage, ClientServiceState clientServiceState)
        {
            AssertNotInactive();

            var serviceClient = ClientService.Create(service, this, cronExpresion, initDateTime, generateInvoice, allowLatePayment, quantity, discountPercentage, clientServiceState);
            Services.Add(serviceClient);
        }

        internal void AssignGroupConcept(GroupConcepts groups, decimal quantity, decimal discountPercentage)
        {
            AssertNotInactive();
            var groupConceptsClient = ClientGroupConcept.Create(groups, this,  quantity,  discountPercentage);
            GroupConcepts.Add(groupConceptsClient);

            
        }

        internal void SetInactive()
        {
            if (State == State.Inactive)
                return;

            State = State.Inactive;
            EventBus.Trigger(new ClientInactiveEvent(this));
        }

        //public void ChangeTipo(Tipos TypeIdentification)
        //{
        //    //TODO In order to fire this event we must to make sure that this change was already applied in the 
        //    //database which means that the transacction was success check unit of work link aspboilerplate
        //    EventBus.Trigger(new ServiceTipoChangedEventData(this.Id, this.TipoId, TypeIdentification.Id));
        //    Tipo = TypeIdentification;
        //}
        private void AssertNotInactive()
        {
            if (State == State.Inactive)
            {
                throw new UserFriendlyException("This client is suspended!");
            }
        }

        public string IdentificationTypeToString()
        {
            string result = string.Empty;
            switch (IdentificationType)
            {
                case IdentificacionTypeTipo.Cedula_Fisica:
                    result = "Cédula Física";
                    break;
                case IdentificacionTypeTipo.Cedula_Juridica:
                    result = "Cédula Jurídica";
                    break;
                case IdentificacionTypeTipo.DIMEX:
                    result = "DIMEX";
                    break;
                case IdentificacionTypeTipo.NITE:
                    result = "NITE";
                    break;
                case IdentificacionTypeTipo.NoAsiganda:
                    result = "Indentificación Extranjera";
                    break;
            }
            return result;
        }

    }

    //public class ServiceTipoChangedEventData : EventData
    //{
    //    public ServiceTipoChangedEventData(Guid clientId, IdentificationType? oldITypeId, IdentificationType? newITypeId)
    //    {
    //        ClienId = clientId;
    //        OldITypeId = oldITypeId;
    //        NewITypeId = newITypeId;
    //    }

    //    public Guid ClienId { get; protected set; }
    //    public IdentificationType? OldITypeId { get; protected set; }
    //    public IdentificationType? NewITypeId { get; protected set; }
    //}

    public enum Gender
    {
        [Description("Femenino")]
        Female,
        [Description("Masculino")]
        Male,
        [Description("Otros")]
        Other
    }

    public enum State
    {
        /// <summary>
        /// El cliente esta Activo / The client is Active
        /// </summary>
        Active,
        /// <summary>
        /// El cliente esta inactivo / The client is inactive
        /// </summary>
        Inactive
    }

    public enum FormaPago
    {
        Dia_Fijo,
        Vencimiento
    }
    public class ClientGroupChangedEventData : EventData
    {
        public ClientGroupChangedEventData(Guid clientId, Guid? oldGroupId, Guid? newGroupId)
        {
            ClientId = clientId;
            OldGroupId = oldGroupId;
            NewGroupId = newGroupId;
        }

        public Guid ClientId { get; protected set; }
        public Guid? OldGroupId { get; protected set; }
        public Guid? NewGroupId { get; protected set; }
    }
}
