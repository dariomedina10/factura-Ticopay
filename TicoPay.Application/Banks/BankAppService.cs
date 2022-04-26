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
using TicoPay.Banks.Dto;
using TicoPay.Invoices;

namespace TicoPay.Banks
{
    public class BankAppService : ApplicationService, IBankAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<Bank, Guid> _bankRepository;
        private readonly IBankManager _bankManager;
        public readonly UserManager _userManager;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public BankAppService(IBankManager bankManager, UserManager userManager, IRepository<Bank, Guid> bankRepository)
        {
            _bankManager = bankManager;
            _userManager = userManager;
            _bankRepository = bankRepository;
        }

        public BankDto Create(CreateBankInput input)
        {
            if (input.Name != null || input.Name != string.Empty)
            {
                var @bank = Bank.Create(AbpSession.GetTenantId(), input.Name, input.ShortName);
                _bankManager.Create(@bank);
                return Mapper.Map<BankDto>(@bank);
            }
            else
            {
                throw new UserFriendlyException("El banco debe tener un nombre.");
            }
        }

        public void Create(CreateBankInput input,int tenantId)
        {
                var @bank = Bank.Create(tenantId, input.Name, input.ShortName);
                _bankManager.Create(@bank);
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
    }
}
