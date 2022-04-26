using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Inventory.Dto;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Inventory
{
    public interface IInventoryAppServices : IApplicationService
    {
        ListResultDto<ProductDto> GetProductos();
        ProductDto Get(Guid input);
        void Update(ProductDto input);
        void Update(UpdateProductInput input);
        ProductDto Create(ProductDto input);
        ProductDto Create(CreateProductInput input);
        ProductDetailOutput GetDetail(Guid input);
        UpdateProductInput GetEdit(Guid input);

        void Delete(Guid input);
        IList<Tax> GetAllTaxes();
        IList<Supplier> GetSuppliers();
        IList<ProductType> GetProductTypes();
        IList<Brand> GetBrands();
        IPagedList<ProductDto> SearchProducts(SearchProductsInput searchInput);
        IList<User> GetAllUser();
        IPagedList<ProductDto> getProducts(SearchProductsInput searchInput);


        //bool isAllowedDelete(Guid Id);
    }
}
