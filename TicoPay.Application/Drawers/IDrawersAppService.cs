using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Drawers.Dto;
using TicoPay.Users;
using TicoPay.BranchOffices;

namespace TicoPay.Drawers
{
    public interface IDrawersAppService : IApplicationService
    {

        //ListResultDto<DrawerDto> GetDrawers();
      
        void Update(DrawerDto input);
        void Update(UpdateDrawerInput input);
        DrawerDto Create(DrawerDto input);
        DrawerDto Create(CreateDrawerInput input);

        DrawerDetailOutput GetDetail(Guid input);
        UpdateDrawerInput GetEdit(Guid input);
        DrawerDto Get(Guid input);

        void Delete(Guid input);

        IPagedList<DrawerDto> SearchDrawers(SearchDrawerInput searchInput);
        ListResultDto<DrawerDto> SearchDrawersApi(SearchDrawerInput searchInput);
        IList<User> GetAllUser();
        IList<BranchOffice> GetBranchOffices();

        bool IsDrawersOpen(Guid Id);
        IList<BranchOffice> getUserbranch();
        Drawer getUserDrawersOpen();
        IList<DrawerUser> getUserDrawers(Guid? IdBranch);
        IList<Drawer> SearchDrawersbyBranch(Guid Id);
        IList<Drawer> GetDrawers(SearchDrawerInput searchInput);
        void OpenDrawer(Guid Id);
        void CloseDrawer(Guid Id);
    }
    
}
