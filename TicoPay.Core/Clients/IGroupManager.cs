using System;
using Abp.Domain.Services;
using TicoPay.MultiTenancy;

namespace TicoPay.Clients
{
    public interface IGroupManager : IDomainService
    {
        Group Get(Guid id);

        void Create(Group @client);

        void ChangeCode(Group @client, Tenant tenant, string code = null);
    }
}
