using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using TicoPay.General;
using TicoPay.MultiTenancy;
using TicoPay.Users;

namespace TicoPay.Invoices
{
    public interface IBankManager : IDomainService
    {
        void Create(Bank @client);

        IList<User> ListUsers();

    }
}
