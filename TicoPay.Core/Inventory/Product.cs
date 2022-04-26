using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using TicoPay.Taxes;
using Abp.Events.Bus;
using TicoPay.Invoices.XSD;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TicoPay.Inventory
{
    public class Product : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the retail price of the product
        /// </summary>
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// Gets or sets the supply price of the product
        /// </summary>
        public decimal SupplyPrice { get; set; }

        /// <summary>
        /// Gets or sets the supply price of the product
        /// </summary>
        public decimal Markup { get; set; }

        /// <summary>
        /// Gets or sets the total in stock
        /// </summary>
        public int TotalInStock { get; set; }

        /// <summary>
        /// Gets or sets default tax id
        /// </summary>
        public Guid TaxId { get; set; }
         
        /// <summary>
        /// Gets or sets default tax
        /// </summary>
        public Tax Tax { get; set; }

        /// <summary>
        /// Gets or sets brand Id
        /// </summary>
        public Guid? BrandId { get; set; }

        /// <summary>
        /// Gets or sets Brand
        /// </summary>
        public virtual Brand Brand { get; set; }

        /// <summary>
        /// Gets or sets brand Id
        /// </summary>
        public Guid? SupplierId { get; set; }

        /// <summary>
        /// Gets or sets Brand
        /// </summary>
        public virtual Supplier Supplier { get; set; }

        /// <summary>
        /// Gets or sets Supplier Code
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// Gets or sets Sales Account Code
        /// </summary>
        public string SalesAccountCode { get; set; }

        /// <summary>
        /// Gets or sets Product Type Id
        /// </summary>
        public Guid? ProductTypeId { get; set; }

        /// <summary>
        /// Gets or sets Product Type
        /// </summary>
        public virtual ProductType ProductType { get; set; }

        /// <summary>Gets or sets the UnitMeasurement. </summary>
        public UnidadMedidaType UnitMeasurement { get; set; }
        /// <summary>
        /// Gets or sets can be sold
        /// </summary>
        public bool CanBeSold { get; set; }

        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Gets or sets Has Variants
        /// </summary>
        public bool HasVariants { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime  CreationTime { get; set; }
        public long? CreatorUserId { get; set; }


        /// <summary>Gets or sets the Tags. </summary>
        public virtual ICollection<ProductTag> Tags { get; set; }

        /// <summary>Gets or sets the Variants. </summary>
        public virtual ICollection<ProductVariant> Variants { get; set; }

        public Estatus Estado { get; set; }

        public static Product Create(int tenantId, string name, decimal retailPrice, UnidadMedidaType unit)
        {
            var @product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                RetailPrice = retailPrice,
                UnitMeasurement = unit,
            };
            return @product;
        }

        /// <summary>
        /// Changes the tax product
        /// </summary>
        /// <param name="tax"></param>
        public void ChangeTax(Tax tax)
        {
            //TODO In order to fire this event we must to make sure that this change was already applied in the 
            //database which means that the transacction was success check unit of work link aspboilerplate
            //EventBus.Trigger(new ProductTaxChangedEventData(this.Id, this.TaxId, tax.Id));
            Tax = tax;
            TaxId = tax.Id;
        }

        public class ProductTaxChangedEventData : EventData
        {
            public ProductTaxChangedEventData(Guid productId, Guid? oldTaxId, Guid? newTaxId)
            {
                ProductId = productId;
                OldTaxId = oldTaxId;
                NewTaxId = newTaxId;
            }

            public Guid ProductId { get; protected set; }
            public Guid? OldTaxId { get; protected set; }
            public Guid? NewTaxId { get; protected set; }
        }

        /// <summary>
        /// Enum que define el estado del producto / Enum that defines the Product State
        /// </summary>
        public enum Estatus
        {
            /// <summary>
            /// Activo / Active
            /// </summary>
            [Description("Activo")]
            Activo,
            /// <summary>
            /// Inactivo / Inactive
            /// </summary>
            [Description("Inactivo")]
            Inactivo
        }
    }
}
