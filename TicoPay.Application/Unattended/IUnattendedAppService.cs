using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.MultiTenancy;
using TicoPay.Unattended.Dto;

namespace TicoPay.Unattended
{
    public interface IUnattendedAppService: IApplicationService
    {
        UnattendedNotification SendXmlTribunet(bool useConsecutive, Tenant tenant, string xmlContent, DateTime dueDate);
        void SyncsUnattendedWithTaxAdministration(Tenant tenant);
        UnattendedStatusToTribunet UnattendedWithTaxAdministration(string voucherKey, Tenant tenant);
    }
}
