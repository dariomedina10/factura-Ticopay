using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;

namespace TicoPay
{
    public class clientService : ITransientDependency
    {
         private readonly IClientAppService _clientservice;
         public clientService(IClientAppService clientservice)
        {
            _clientservice = clientservice;
        }

         public ClientBN GetExistClientByCode(TipoLLaveAcceso tipo, long code)
         {
             return  _clientservice.GetExistClientByCode(tipo, code);
         }

         public void UpdateClientBn(ClientBN client)
         {
             _clientservice.UpdateClientBn(client);
         }
        /// <summary>
        /// tester client
        /// </summary>
        /// <returns></returns>
         public Client GetClient()
         {
             return _clientservice.GetClient(Guid.Parse("d2e7814e-ab00-4d19-8b73-a55d1917651c"));
         }
    }
}
