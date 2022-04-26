using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.MultiTenancy;

namespace TicoPay.ConsultaRecibos
{
    public interface IConsultaReciboAppService : IApplicationService
    {
        IList<Tenant> GetAllTenants(); 
    }
}
