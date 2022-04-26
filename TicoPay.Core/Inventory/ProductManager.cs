using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using TicoPay.Taxes;

namespace TicoPay.Inventory
{
    public class ProductManager : DomainService, IProductManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Tax, Guid> _taxRepository;

        public ProductManager(IRepository<Product, Guid> productRepository, IRepository<Tax, Guid> taxRepository)
        {
            _productRepository = productRepository;
            _taxRepository = taxRepository;
            EventBus = NullEventBus.Instance;
        }

        //public ServiceManager

        public void AssignTaxToProduct(Tax tax, Product product)
        {
            if (tax == null || product.TaxId == tax.Id)
            {
                return;
            }
            product.ChangeTax(tax);
        }

        public IList<Tax> GetAllListTaxes()
        {
            return _taxRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
        }

        public Product Get(Guid ProductId)
        {
            return _productRepository.GetAll().Include(a => a.Tax).Where(a => a.Id == ProductId).FirstOrDefault();
        }

    }
}
