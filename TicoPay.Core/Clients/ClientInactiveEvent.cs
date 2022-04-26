using Abp.Events.Bus.Entities;

namespace TicoPay.Clients
{
    public class ClientInactiveEvent : EntityEventData<Client>
    {
        public ClientInactiveEvent(Client entity)
            : base(entity)
        {
        }
    }
}
