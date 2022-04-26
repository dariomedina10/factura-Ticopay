using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.UI;
using System;
using System.Collections.Generic;
using TicoPay.Taxes;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using TicoPay.General;
using System.Data.Entity;
using System.Linq;
using TicoPay.Invoices;

namespace TicoPay.Taxes
{
    public class BankManager : DomainService, IBankManager
    {

        public IEventBus EventBus { get; set; }
        private readonly IRepository<Bank, Guid> _bankRepository;
        

        public BankManager(IRepository<Bank, Guid> BankRepository)
        {
            _bankRepository = BankRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(Bank @Bank)
        {
            _bankRepository.Insert(@Bank);
        }

        public IList<User> ListUsers()
        {
            throw new NotImplementedException();
        }
    }
}
