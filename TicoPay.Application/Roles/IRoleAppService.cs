using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Roles.Dto;
using Abp.Authorization.Users;
using PagedList;
using System.Collections.Generic;
using TicoPay.Users;
using Abp.Authorization;
using Abp.Application.Services.Dto;

namespace TicoPay.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
        int GetRoleByUser(long IdUser);
        IPagedList<RolesDTO> SearchRoles(SearchRolesInput searchInput);

        IList<UserRole> GetAllListUserRoles();

        RolDetailOutput GetDetail(int input);
        IList<User> GetAllUser();
        List<PermissionListDto> GetAllPermission();

        Task<bool> Create(CreateRoleInput input);
        Task Update(UpdateRoleInput input);
        ListResultDto<RolesDTO> GetAllRoles();
        ListResultDto<UserRoleDto> GetUserRoles(long UserId);
        Task<ListResultDto<PermissionListDto>> getPermissions(long UserId);
    }
}
