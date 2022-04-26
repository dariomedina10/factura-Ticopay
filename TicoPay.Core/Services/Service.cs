using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Abp.Events.Bus;
using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;
using TicoPay.Taxes;
using TicoPay.Clients;
using TicoPay.Invoices.XSD;
using TicoPay.Invoices;

//this is a test borrar esto despues
namespace TicoPay.Services
{
    /// <summary>
    /// Represents a service entity
    /// </summary>
    public class Service : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public const int MaxNameLength = 160;

        public string CronExpression { get; set; }

        /// <summary>Gets or sets the Name. </summary>
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        private decimal _price = 0;

        public UnidadMedidaType UnitMeasurement { get; set; }

        public string UnitMeasurementOthers { get; set; }

        /// <summary>Gets or sets the Price. </summary>
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (value < 0)
                {
                    //TODO: Use Language Management http://www.aspnetboilerplate.com/Pages/Documents/Zero/Language-Management
                    throw new ApplicationException("Price cannot be a negative value");
                }
                _price = value;
            }
        }

        public bool IsRecurrent { get; set; }

        /// <summary>
        /// Gets or sets tax id
        /// </summary>
        public Guid? TaxId { get; protected set; }

        /// <summary>
        /// Gets or sets tax
        /// </summary>
        public Tax Tax { get; protected set; }

        /// <summary>
        /// Gets or sets the Client List
        /// </summary>
        /// 
        public virtual ICollection<ClientService> Clients { get; protected set; }

        public IEventBus EventBus { get; set; }

        public int TenantId { get; set; }

        public decimal Quantity { get; set; }

        public decimal DiscountPercentage { get; set; }

        protected Service()
        {
            EventBus = NullEventBus.Instance;
        }

        //public Service(string name, decimal price, Tax tax = null)
        //{
        //    Name = name;
        //    Price = price;
        //    Tax = tax;
        //}

        public static Service Create(int tenantId, string name, decimal price, string cronExpression, UnidadMedidaType unit, string unitothers, bool isRecurrent, decimal quantity, decimal discountPercentage)
        {
            var @service = new Service
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                CronExpression = cronExpression,
                Price = price,
                UnitMeasurement = unit,
                UnitMeasurementOthers = unitothers,
                IsRecurrent = isRecurrent,
                Quantity=quantity,
                DiscountPercentage=discountPercentage
                
            };
            //@service.Taxs = new Collection<ClientService>();
            //@client.Invoices = new Collection<Invoice>();
            return @service;
        }
        /// <summary>
        /// Changes the tax service
        /// </summary>
        /// <param name="tax"></param>
        public void ChangeTax(Tax tax)
        {
            //TODO In order to fire this event we must to make sure that this change was already applied in the 
            //database which means that the transacction was success check unit of work link aspboilerplate
            EventBus.Trigger(new ServiceTaxChangedEventData(this.Id, this.TaxId, tax.Id));
            Tax = tax;
        }

        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public long? DeleterUserId { get; set; }

        public virtual ICollection<GroupConcepts> GroupConcepts { get; set; }
    }

    public class ServiceTaxChangedEventData : EventData
    {
        public ServiceTaxChangedEventData(Guid serviceId, Guid? oldTaxId, Guid? newTaxId)
        {
            ServiceId = serviceId;
            OldTaxId = oldTaxId;
            NewTaxId = newTaxId;
        }

        public Guid ServiceId { get; protected set; }
        public Guid? OldTaxId { get; protected set; }
        public Guid? NewTaxId { get; protected set; }
    }

    public enum SelectRecurrent
    {
        Recurrent,
        NonRecurring
    }
}
