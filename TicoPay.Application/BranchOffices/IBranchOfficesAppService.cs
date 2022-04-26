using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices.Dto;
using TicoPay.Users;
using PagedList;


namespace TicoPay.BranchOffices
{
    public interface IBranchOfficesAppService: IApplicationService
    {
        ListResultDto<BranchOfficesDto> GetBranchOffices();
        BranchOfficesDto Get(Guid input);
        BranchOfficesDetailOutput GetDetail(Guid input);
        IList<BranchOffice> GetServicesEntities();
        IList<BranchOffice> GetServicesEntities(int TenantId);
        void Update(UpdateBranchOfficesInput input);
        void Update(BranchOfficesDto input);
        BranchOfficesDto Create(CreateBranchOfficesInput input);
        BranchOfficesDto Create(BranchOfficesDto input);
        void Delete(Guid input);
        UpdateBranchOfficesInput GetEdit(Guid input);

        IPagedList<BranchOfficesDto> SearchServices(SearchBranchOfficesInput searchInput);

        bool isAllowedDelete(Guid Id);
        IList<User> GetAllUser();
    }
}
