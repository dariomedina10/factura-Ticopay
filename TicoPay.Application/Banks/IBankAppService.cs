using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
//using PagedList;
using TicoPay.Services;
using TicoPay.Taxes.Dto;
using TicoPay.Users;
using TicoPay.Banks.Dto;
using TicoPay.Invoices;
using PagedList;
using System.Linq.Expressions;

namespace TicoPay.Banks
{
    public interface IBankAppService : IApplicationService
    {
        BankDto Create(CreateBankInput input);
        void Create(CreateBankInput input, int tenantId);
        IList<User> GetAllUser();
    }
}
