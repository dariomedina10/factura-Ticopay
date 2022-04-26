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

namespace TicoPay.Taxes
{
    public class TaxManager : DomainService, ITaxManager
    {

        public IEventBus EventBus { get; set; }
        private readonly IRepository<Tax, Guid> _taxRepository;
        

        public TaxManager(IRepository<Tax, Guid> TaxRepository)
        {
            _taxRepository = TaxRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(Tax @Tax)
        {
            _taxRepository.Insert(@Tax);
        }

        public IList<User> ListUsers()
        {
            throw new NotImplementedException();
        }

        public Tax Get(Guid id)
        {
            var @Tax = _taxRepository.FirstOrDefault(id);
            if (@Tax == null)
            {
                throw new UserFriendlyException("Could not found the Service, maybe it's deleted!");
            }

            return @Tax;
        }


    }
}
