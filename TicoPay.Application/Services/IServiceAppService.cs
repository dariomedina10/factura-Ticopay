using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Invoices;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Services
{
    public interface IServiceAppService : IApplicationService
    {
        ListResultDto<ServiceDto> GetServices();
        ServiceDto Get(Guid input);
        ServiceDetailOutput GetDetail(Guid input);
        IList<Service> GetServicesEntities();
        IList<Service> GetServicesEntities(int TenantId);
        void Update(UpdateServiceInput input);
        void Update(ServiceDto input);
        ServiceDto Create(CreateServiceInput input);
        ServiceDto Create(ServiceDto input);
        void Delete(Guid input);
        UpdateServiceInput GetEdit(Guid input);
        IPagedList<ServiceDto> SearchServices(SearchServicesInput searchInput);
        IPagedList<ServiceDto> getServices(SearchServicesInput searchInput);
        IList<Tax> GetAllTaxes();
        IList<User> GetAllUser();
        IList<ClientService> GetAllClientServices();
        bool isAllowedDelete(Guid Id);

        Service CreateServiceInvoice(CreateServiceInput input);
        Service GetBy(Expression<Func<Service, bool>> predicate);

       IList<InvoiceLine> invoiceLineRepository();

 
    }
}
