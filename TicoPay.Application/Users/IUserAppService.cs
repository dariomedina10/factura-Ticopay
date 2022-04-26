using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Users.Dto;
using System.Collections.Generic;
using TicoPay.Drawers;
using System;

namespace TicoPay.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(ProhibitPermissionInput input);

        Task RemoveFromRole(long userId, string roleName);

        Task<ListResultDto<UserListDto>> GetUsers();

        Task CreateUser(CreateUserInput input);

        IPagedList<UserListDto> SearchUsers(SearchUsersInput searchInput);

        Task Update(UpdateUserInput input);

        bool UserNameExist(string userName);
        User GetUserByRole(string name);

        User GetUserByRole(string name, int tenantId);

        bool EmailExistCreate(string userName);

        void ChangeStatus(long id);

        IList<User> GetAllListUsersSoftDelete();

        Task UpdateDrawesUser(UpdateUserDrawers input);

        IList<DrawerUser> getUserDrawers(Guid? IdBranch, Guid? IdDrawer = null);
    }
}