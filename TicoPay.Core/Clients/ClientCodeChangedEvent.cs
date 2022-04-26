using Abp.Events.Bus.Entities;

namespace TicoPay.Clients
{
    public class ClientCodeChangedEvent : EntityEventData<Client>
    {
        public ClientCodeChangedEvent(Client entity)
            : base(entity)
        {
        }
    }
}
