using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using PagedList;
using TicoPay.Inventory.Dto;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Inventory
{
    public class InventoryAppServices : ApplicationService, IInventoryAppServices
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<InvoiceLine, Guid> _invoiceLineRepository;
        private readonly IRepository<ProductType, Guid> _productTypeRepository;
        private readonly IServiceManager _serviceManager;
        private readonly IProductManager _productManager;
        private readonly IRepository<Brand, Guid> _brandRepository;
        public readonly UserManager _userManager;
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Tenant, int> _tenantRepository;

        public InventoryAppServices(
            IRepository<Product,Guid> productRepository,
            IServiceManager serviceManager,
            IProductManager productManager,
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<InvoiceLine, Guid> invoiceLinesRepository,
            IRepository<ProductType, Guid> productTypeRepository,
            IRepository<Brand, Guid> brandRepository,
            UserManager userManager, IRepository<Tax, Guid> taxRepository, TenantManager tenantManager, IRepository<Tenant, int> tenantRepository
            )
        {
            _productRepository = productRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLinesRepository;
            _serviceManager = serviceManager;
            _productManager = productManager;
            _supplierRepository = supplierRepository;
            _productTypeRepository = productTypeRepository;
            _brandRepository = brandRepository;
            _userManager = userManager;
            _taxRepository = taxRepository;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
        }

        //public ProductDto Create(ProductDto input)
        //{
        //    throw new NotImplementedException();

        //}

        [UnitOfWork]
        [Abp.Runtime.Validation.DisableValidation]
        public ProductDto Create(ProductDto input)
        {

            var tenantId = AbpSession.GetTenantId();

            Product @product = Product.Create(tenantId, input.Name, input.RetailPrice, input.UnitMeasurement);
            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId);
                product.TaxId = input.TaxId;
                _productManager.AssignTaxToProduct(@tax, @product);
            }

            var checkProduct = _productRepository.GetAll().Where(x => x.Name == @product.Name && x.TenantId == @product.TenantId).FirstOrDefault();
            if (checkProduct != null){
                throw new UserFriendlyException("Existe un producto con el mismo Nombre. \n");}

            _productRepository.Insert(@product);
            
            return Mapper.Map<ProductDto>(@product);
        }

        public void Delete(Guid input)
        {
            var tenantId = AbpSession.GetTenantId();
            var productId = input;
            var checkProducto = _invoiceLineRepository.GetAll().Count(x => x.ProductId == productId && x.TenantId == tenantId);

            if (checkProducto > 0)
            {
                throw new UserFriendlyException("No se puede eliminar el Producto. Este producto esta asociado a una factura!.");
            }


            var product =  _productRepository.Get(input);
            if (product == null)
            {
                throw new UserFriendlyException("Could not found the product, maybe it's deleted.");
            }

            product.IsDeleted = true;
            _productRepository.Update(product);
        }

        public ProductDto Get(Guid input)
        {
            throw new NotImplementedException();
        }

        public IList<Tax> GetAllTaxes()
        {
            return _serviceManager.GetAllListTaxes();
           
        }

        public UpdateProductInput GetEdit(Guid input)
        {
            var product = _productRepository.GetAll().Include(a => a.Tax)
                                                    //.Include(s => s.ProductType)
                                                    //.Include(b => b.Brand)
                                                    //.Include(sp => sp.Supplier)
                                                    .Where(a => a.Id == input).FirstOrDefault();
            if (product == null)
            {
                throw new UserFriendlyException("Could not found the Product, maybe it's deleted.");
            }
            return product.MapTo<UpdateProductInput>();
        }

        public IList<Supplier> GetSuppliers()
        {
            return _supplierRepository.GetAll().ToList();
        }
        public IList<ProductType> GetProductTypes()
        {
            return _productTypeRepository.GetAll().ToList();
        }


        public ListResultDto<ProductDto> GetProductos()
        {
            var tenantId = AbpSession.GetTenantId();
            var products = _productRepository.GetAll()
                    .Include(a => a.Tax)
                    .Where(c => c.TenantId == tenantId && c.Estado == Product.Estatus.Activo);            
            return new ListResultDto<ProductDto > (products.MapTo<List <ProductDto>> ());
        }

        public IPagedList<ProductDto> SearchProducts(SearchProductsInput searchInput)
        {
            var products = _productRepository.GetAll().Include(a => a.Tax);
            var tenantId = AbpSession.GetTenantId();
            var checkProducto = _invoiceLineRepository.GetAll().Where(x => x.TenantId == tenantId);

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;
          
            if (!String.IsNullOrEmpty(searchInput.NameFilter))
                products = products.Where(c => c.Name.ToLower().Contains(searchInput.NameFilter)
                || c.Name.ToLower().Equals(searchInput.NameFilter));

           if (searchInput.BrandFilter != null)
                products = products.Where(c => c.BrandId == searchInput.BrandFilter);

            if (searchInput.TaxFilter != null)
                products = products.Where(c => c.TaxId == searchInput.TaxFilter);

            if (searchInput.PriceSinceFilter != null)
                products = products.Where(c => c.RetailPrice >= searchInput.PriceSinceFilter);

            if (searchInput.PriceUntilFilter != null)
                products = products.Where(c => c.RetailPrice <= searchInput.PriceUntilFilter);

            if (searchInput.EstadoFilter != null)
                products = products.Where(c => c.Estado == searchInput.EstadoFilter);

            //return products.MapTo<List<ProductDto>>().OrderBy(p => p.Name).ToList().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
            var listProduct = (from p in products
                               select new ProductDto
                               {
                                   Id = p.Id,
                                   Name = p.Name,
                                   RetailPrice = p.RetailPrice,
                                   UnitMeasurement = p.UnitMeasurement,
                                   TaxId = p.TaxId,
                                   Tax = p.Tax==null? null: new Taxes.Dto.TaxDto { Id = p.Tax.Id, Name = p.Tax.Name, Rate = p.Tax.Rate, TaxTypes = p.Tax.TaxTypes },
                                   Estado = p.Estado,
                                   CanBeDeleted = (checkProducto.Any(x => x.ProductId == p.Id && x.TenantId == tenantId) ? 1 : 0)
                               }).OrderBy(p => p.Name).ToPagedList(currentPageIndex, searchInput.MaxResultCount);
            return listProduct;
        }

        public void Update(ProductDto input)
        {
            throw new NotImplementedException();
        }

        public void Update(UpdateProductInput input)
        {

            var @product = _productRepository.Get(input.Id);
            if (@product == null)
            {
                throw new UserFriendlyException("No se Puede Encontrar El Producto");
            }

            @product.Name = input.Name;
            @product.UnitMeasurement = input.UnitMeasurement;
            @product.Estado = input.Estado;
            //@product.BrandId = input.BrandId;
            //@product.ProductTypeId = input.ProductTypeId;
            //@product.SupplierId = input.SupplierId;
            @product.TaxId = input.TaxId;
            //@product.TotalInStock = input.TotalInStock;
            //@product.SalesAccountCode = input.SalesAccountCode;
            @product.RetailPrice = input.RetailPrice;
            //@product.SupplyPrice = input.SupplyPrice;

            _productRepository.Update(@product);
        }

        public IList<Brand> GetBrands()
        {
            return _brandRepository.GetAll().ToList();
        }

        public ProductDetailOutput GetDetail(Guid input)
        {
            //var @service = _serviceRepository.GetAll().Include(a => a.Tax).Where(a => a.Id == input).FirstOrDefault();

            var @product = _productRepository.GetAll().Include(a => a.Tax)
                                                     //.Include(s => s.ProductType)
                                                     //.Include(b => b.Brand)
                                                     //.Include(sp => sp.Supplier)
                                                     .Where(a => a.Id == input).FirstOrDefault();

            if (@product == null)
            {
                throw new UserFriendlyException("Could not found the Product, maybe it's deleted.");
            }

            return  @product.MapTo<ProductDetailOutput>();
        }

        public ProductDto Create(CreateProductInput input)
        {
            var currentTenant = _tenantManager.Get(AbpSession.GetTenantId());

            var product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = AbpSession.GetTenantId(),
                Name = input.Name,
                //BrandId = input.BrandId,
                //SupplierId = input.SupplierId,
                TaxId = input.TaxId,
                //ProductTypeId = input.ProductTypeId,
                RetailPrice = input.RetailPrice,
                //SupplyPrice = input.SupplyPrice,
                Estado= input.Estado,
                UnitMeasurement=input.UnitMeasurement.Value
               
            };

            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId);
                AssignTaxToProduct(tax, product);
            }

            _productRepository.Insert(product);

            if (!currentTenant.IsTutorialProduct)
            {
                currentTenant.IsTutorialProduct = true;
                currentTenant.IsTutotialServices = true;
                _tenantRepository.Update(currentTenant);
            }

            return product.MapTo<ProductDto>();
        }

        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
            {
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            }
            return users;
        }

        public IPagedList<ProductDto> getProducts(SearchProductsInput searchInput)
        {
            if (searchInput.NameFilter == null || searchInput.NameFilter == "")
                searchInput.NameFilter = "";
            else
                searchInput.NameFilter = searchInput.NameFilter.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            var products = _productRepository.GetAll()
                    .Include(a => a.Tax)
                    .Where(c => c.Name.ToLower().Contains(searchInput.NameFilter)
                    || c.Name.ToLower().Equals(searchInput.NameFilter));

            products = products.Where(c => c.Estado == Product.Estatus.Activo);

            if (searchInput.PriceSinceFilter != null)
                products = products.Where(c => c.RetailPrice >= searchInput.PriceSinceFilter);

            if (searchInput.PriceUntilFilter != null)
                products = products.Where(c => c.RetailPrice <= searchInput.PriceUntilFilter);

            if (searchInput.TaxId != null)
                products = products.Where(c => c.TaxId == searchInput.TaxId);

            var listProduct = (from p in products
                               select new ProductDto
                              {
                                  Id = p.Id,
                                  Name = p.Name,
                                  RetailPrice = p.RetailPrice,
                                  UnitMeasurement = p.UnitMeasurement,
                                  TaxId = p.TaxId,
                                  Tax = new Taxes.Dto.TaxDto { Id = p.Tax.Id, Name = p.Tax.Name, Rate = p.Tax.Rate, TaxTypes = p.Tax.TaxTypes }
                                  
                               }).OrderBy(p => p.Name).ToPagedList(currentPageIndex, searchInput.MaxResultCount);
            return listProduct;
        }

        private void AssignTaxToProduct(Tax tax, Product product)
        {
            if (tax == null || product.TaxId == tax.Id)
            {
                return;
            }
            product.ChangeTax(tax);
        }
    }
}
