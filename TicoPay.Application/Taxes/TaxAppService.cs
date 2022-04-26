using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper;
using System;
using Abp.Application.Services.Dto;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.AutoMapper;
using Abp.Runtime.Session;
using System.Data;
using System.Data.Entity;
using System.Web.WebPages;
using Abp.UI;
//using PagedList;
using TicoPay.Services;
using TicoPay.Taxes.Dto;
using TicoPay.Users;
using TicoPay.General;
using PagedList;
using System.Linq.Expressions;
using TicoPay.Inventory;

namespace TicoPay.Taxes
{
    public class TaxAppService : ApplicationService, ITaxAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ITaxManager _taxManager;
        public readonly UserManager _userManager;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public TaxAppService(IRepository<Tax, Guid> taxRepository, ITaxManager taxManager, UserManager userManager,
            IRepository<Service, Guid> serviceRepository, IRepository<Product, Guid> productRepository)
        {
            _taxRepository = taxRepository;
            _taxManager = taxManager;
            _userManager = userManager;
            _serviceRepository = serviceRepository;
            _productRepository = productRepository;
        }

        public TaxDto Create(CreateTaxInput input)
        {
            if (input.Rate.ToString().IsDecimal())
            {
                decimal temp = Convert.ToDecimal(input.Rate);
                var @tax = Tax.Create(AbpSession.GetTenantId(), input.Name, temp, input.TaxTypes);
                _taxManager.Create(@tax);
                return Mapper.Map<TaxDto>(@tax);
            }
            else
            {
                throw new UserFriendlyException("El porcentaje solo debe contener números.");
            }

            
        }

        public void Create(CreateTaxInput input,int tenantId)
        {
            if (input.Rate.ToString().IsDecimal())
            {
                decimal temp = Convert.ToDecimal(input.Rate);
                var @tax = Tax.Create(tenantId, input.Name, temp, input.TaxTypes);
                _taxManager.Create(@tax);
            }
            else
            {
                throw new UserFriendlyException("El porcentaje solo debe contener numeros.");
            }
        }

        public ListResultDto<TaxDto> GetTaxes()
        {
            try
            {
                var taxes = _taxRepository.GetAllList().OrderBy(a => a.Name);              
                return new ListResultDto<TaxDto>(taxes.MapTo<List<TaxDto>>());
            }
            catch 
            {
                return null;
            }
           
        }

        public TaxDetailOutput GetDetail(Guid input)
        {
            var @taxes = _taxRepository.FirstOrDefault(input);

            if (@taxes == null)
            {
                throw new UserFriendlyException("Could not found the tax, maybe it's deleted.");
            }

            return @taxes.MapTo<TaxDetailOutput>();
        }

        public TaxDto Get(Guid input)
        {
            try
            {
                var @tax = _taxRepository.Get(input);              
                return Mapper.Map<TaxDto>(@tax);
            }
            catch 
            {
                return null;
            }
            
        }

        public UpdateTaxInput GetEdit(Guid input)
        {
            var @tax = _taxRepository.Get(input);
            if (@tax == null)
            {
                throw new UserFriendlyException("Could not found the tax, maybe it's deleted.");
            }
            UpdateTaxInput view = new UpdateTaxInput();
            view.Id = @tax.Id;
            view.Rate = @tax.Rate;
            view.Name = @tax.Name;
            view.TenantId = @tax.TenantId;
            view.TaxTypes = @tax.TaxTypes;
            return view;
        }

        /// <summary>
        /// Searches for taxes and returns page result
        /// </summary>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public IPagedList<TaxDto> SearchTaxes(SearchTaxesInput searchInput)
        {
            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value  : 1;
            var taxes = _taxRepository.GetAll().Where(c => c.Name.ToLower().Contains(searchInput.Query) || c.Name.ToLower().Equals(searchInput.Query)).OrderByDescending(p => p.Name).ToList();
            if (taxes == null)
            {
                throw new UserFriendlyException("Could not found the tax, maybe it's deleted.");
            }            
            
            return taxes.MapTo<List<TaxDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);

        }

        public void Update(UpdateTaxInput input)
        {
            var @tax = _taxRepository.Get(input.Id);
            if (@tax == null)
            {
                throw new UserFriendlyException("Could not found the tax, maybe it's deleted.");
            }

            @tax.Name = input.Name;
            decimal temp = Convert.ToDecimal(input.Rate);
            @tax.Rate = temp;
            @tax.TaxTypes = input.TaxTypes;

        }


        public void Delete(Guid id)
        {
          
            if (isAllowedDelete(id))
            {
                var @tax = _taxRepository.Get(id);
                @tax.IsDeleted = true;
                _taxRepository.Update(@tax);
            }
            else {
                throw new UserFriendlyException("No se puede eliminar un impuesto asociado a un producto o servicio.");
            }
           
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

        public IList<Service> GetAllListServices()
        {
            var services = _serviceRepository.GetAllList();
            if (services == null)
            {
                throw new UserFriendlyException("Could not found the services, maybe it's deleted.");
            }
            return services;
        }

        public bool isAllowedDelete(Guid Id)
        {          
            var services = _serviceRepository.GetAll().Where(x=>x.TaxId== Id).Count();
            var product = _productRepository.GetAll().Where(x => x.TaxId == Id).Count();
            return ((services <= 0)&& (product <= 0)) ?true: false;


        }

        public Tax GetBy(Expression<Func<Tax, bool>> predicate)
        {
            return _taxRepository.GetAll().Where(predicate).FirstOrDefault();
        }
    }
}
