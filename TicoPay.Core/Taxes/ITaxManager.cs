using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using TicoPay.General;
using TicoPay.MultiTenancy;
using TicoPay.Users;

namespace TicoPay.Taxes
{
    public interface ITaxManager : IDomainService
    {
        Tax Get(Guid id);

        void Create(Tax @client);

        IList<User> ListUsers();

    }
}
