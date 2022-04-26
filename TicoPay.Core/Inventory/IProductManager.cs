using System.Collections.Generic;
using Abp.Domain.Services;
using TicoPay.Taxes;
using System;


namespace TicoPay.Inventory
{
    public interface IProductManager : IDomainService
    {
        void AssignTaxToProduct(Tax tax, Product product);
        IList<Tax> GetAllListTaxes();
        Product Get(Guid ProductId);
    }
}
