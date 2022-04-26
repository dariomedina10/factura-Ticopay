using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.UI;
using System;
using TicoPay.General;
using TicoPay.MultiTenancy;

namespace TicoPay.Clients
{
    public class GroupManager : DomainService, IGroupManager
    {

        public IEventBus EventBus { get; set; }
        private readonly IRepository<Group, Guid> _groupRepository;
        //private readonly ICodeGenerator _codeGenerator;

        public GroupManager(IRepository<Group, Guid> groupRepository)
        {
            _groupRepository = groupRepository;
            EventBus = NullEventBus.Instance;
           // _codeGenerator = codeGenerator;
        }
       
        public void Create(Group @group)
        {
            _groupRepository.Insert(@group);
        }

        public Group Get(Guid id)
        {
            var @group =  _groupRepository.FirstOrDefault(id);
            if (@group == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted!");
            }

            return @group;
        }

        public void ChangeCode(Group @group, Tenant tenant, string code = null)
        {
            //@group.ChangeCode(tenant, _codeGenerator, code);
            throw new NotImplementedException();
        }

        Group IGroupManager.Get(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
