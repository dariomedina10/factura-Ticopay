using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using TicoPay.Clients;
using TicoPay.ReportClients.Dto;

namespace TicoPay.ReportClients
{
    public class ReportClientsAppService : ApplicationService, IReportClientsAppService
    {
        private readonly IRepository<Client, Guid> _clientRepository;

        public ReportClientsAppService(IRepository<Client, Guid> clientRepository)
        {
            _clientRepository = clientRepository;
        }


        public IList<Client> SearchReportClients(ReportClientsInputDto searchInput)
        {
            var query = "";

            var incoiceList = _clientRepository.GetAll();

            if (searchInput.Query != "" && searchInput.Query!=null)
            {
                query = searchInput.Query.ToLower();
                
                incoiceList = incoiceList.Where(a => a.Name.ToLower().Contains(query) || a.LastName.ToLower().Contains(query) || a.Code.ToString().Contains(query)
                || a.Identification.Contains(query) || a.Groups.Any(g=>g.Group.Name.ToLower().Contains(query)));
                
            }


            return incoiceList.Include(a => a.Services).Include(a => a.Groups).OrderBy(a => a.Name).ToList();
            //incoiceList.Include(a => a.Services).Include(a => a.Groups).OrderBy(a => a.Name).ToList();
        }
    }
}
