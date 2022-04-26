using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
//using PagedList;
using TicoPay.Services;
using TicoPay.Taxes.Dto;
using TicoPay.Users;
using TicoPay.General;
using PagedList;
using System.Linq.Expressions;

namespace TicoPay.Taxes
{
    public interface ITaxAppService : IApplicationService
    {
        ListResultDto<TaxDto> GetTaxes();
        TaxDto Get(Guid input);
        UpdateTaxInput GetEdit(Guid input);
        TaxDetailOutput GetDetail(Guid input);
        void Update(UpdateTaxInput input);
        TaxDto Create(CreateTaxInput input);
        void Create(CreateTaxInput input, int tenantId);
        void Delete(Guid id);
        IPagedList<TaxDto> SearchTaxes(SearchTaxesInput searchInput);
        IList<User> GetAllUser();
        IList<Service> GetAllListServices();
        bool isAllowedDelete(Guid Id);
        Tax GetBy(Expression<Func<Tax, bool>> predicate);
    }
}
