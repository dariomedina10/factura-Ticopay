using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices.Dto;
using TicoPay.Drawers;
using Abp.AutoMapper;
using PagedList;
using TicoPay.Users;

namespace TicoPay.BranchOffices
{
    public class BranchOfficesAppService : ApplicationService, IBranchOfficesAppService 
    {
        private readonly IRepository<BranchOffice, Guid> _branchOfficesRepository;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        public readonly UserManager _userManager;
             
        public BranchOfficesAppService(
            IRepository<BranchOffice, Guid> branchOfficesRepository, IRepository<Drawer, Guid> drawerRepository,
        UserManager userManager)
        {
            _branchOfficesRepository = branchOfficesRepository;
            _userManager = userManager;
            _drawerRepository = drawerRepository;
        }

        public BranchOfficesDto Create(CreateBranchOfficesInput input)
        {
            var tenantId = AbpSession.GetTenantId();


            BranchOffice branchOffice = BranchOffice.Create(tenantId, input.Name, input.Code, input.Location);
           
            var checkBranch = _branchOfficesRepository.GetAll().Where(x => x.Code == branchOffice.Code && x.TenantId == branchOffice.TenantId).FirstOrDefault();
            var checkName = _branchOfficesRepository.GetAll().Where(x => x.Name == branchOffice.Name && x.TenantId == branchOffice.TenantId).FirstOrDefault();

            if (checkBranch != null){
                throw new UserFriendlyException("Existe una sucursal con el mismo número de Código");}

            if (checkName != null){
                throw new UserFriendlyException("Existe una sucursal con el mismo Nombre");}

            _branchOfficesRepository.Insert(branchOffice);

            return branchOffice.MapTo<BranchOfficesDto>();
        }

        public BranchOfficesDto Create(BranchOfficesDto input)
        {
            var tenantId = AbpSession.GetTenantId();

            input.Name = DataName(input.Name);
            input.Code = DataCode(input.Code);

            var checkBranch = _branchOfficesRepository.GetAll().Where(x => x.Code == input.Code && x.TenantId == tenantId).FirstOrDefault();
            var checkName = _branchOfficesRepository.GetAll().Where(x => x.Name == input.Name && x.TenantId == tenantId).FirstOrDefault();

            if (checkBranch != null)
            {
                throw new UserFriendlyException("Existe una sucursal con el mismo número de Código. \n");
            }

            if (checkName != null)
            {
                throw new UserFriendlyException("Existe una sucursal con el mismo Nombre. \n");
            }

            BranchOffice branchOffice = BranchOffice.Create(AbpSession.GetTenantId(), input.Name, input.Code, input.Location);
          
            _branchOfficesRepository.Insert(branchOffice);

            return branchOffice.MapTo<BranchOfficesDto>();
        }
        public string DataName(string Name)
        {
            string newName = "";
            if (Name.Length > 50) { newName = Name.Substring(0, 49); } else { newName = Name; }
            return newName;
        }

        public string DataCode(string Code)
        {
            string newCode = "";
            newCode = Code.PadLeft(3, '0');

            return newCode;
        }
        public void Delete(Guid input)
        {
            var tenantId = AbpSession.GetTenantId();
            var branchId = input;
           // Drawer drawer = new Drawer();
            //BranchOffice branchDrawer = new BranchOffice();

            var checkSucursal = _drawerRepository.GetAll().Where(x => x.BranchOfficeId == branchId && x.TenantId == tenantId).FirstOrDefault();

            if (checkSucursal != null) {
                throw new UserFriendlyException("No se puede eliminar. Esta sucursal tiene cajas asociadas!.");
            }
            var branchOffice = _branchOfficesRepository.Get(input);
            if (branchOffice == null)
            {
                throw new UserFriendlyException("Could not found the branchOffice, maybe it's deleted."); 
            }
            branchOffice.IsDeleted = true;
            _branchOfficesRepository.Update(branchOffice);
        }

        public BranchOfficesDto Get(Guid input)
        {
            var branchOffice = _branchOfficesRepository.GetAll().Where(x => x.Id == input).FirstOrDefault();

            return branchOffice.MapTo<BranchOfficesDto>();
        }

        public ListResultDto<BranchOfficesDto> GetBranchOffices()
        {
            var branchOffice = _branchOfficesRepository.GetAll().ToList();
            if (branchOffice== null)
            {
                throw new UserFriendlyException("Could not found the Branch, maybe it's deleted.");
            }

            return new ListResultDto<BranchOfficesDto>(branchOffice.MapTo<List<BranchOfficesDto>>());
        }

        public BranchOfficesDetailOutput GetDetail(Guid input)
        {
            var branchOffice = _branchOfficesRepository.GetAll().Where(a => a.Id == input).FirstOrDefault();

            if (branchOffice == null)
            {
                throw new UserFriendlyException("Sucursal No Encontrada");
            }

            return branchOffice.MapTo<BranchOfficesDetailOutput>();
        }

        public UpdateBranchOfficesInput GetEdit(Guid input)
        {
            var branchOffice = _branchOfficesRepository.GetAll().Where(a => a.Id == input).ToList().FirstOrDefault();
            if (branchOffice == null)
            {
                throw new UserFriendlyException("Sucursal No Encontrada");
            }

            return branchOffice.MapTo<UpdateBranchOfficesInput>();
        }

        public IList<BranchOffice> GetServicesEntities()
        {
            throw new NotImplementedException();
        }

        public IList<BranchOffice> GetServicesEntities(int TenantId)
        {
            throw new NotImplementedException();
        }

        public bool isAllowedDelete(Guid Id)
        {


            throw new NotImplementedException();
        }

        public IPagedList<BranchOfficesDto> SearchServices(SearchBranchOfficesInput searchInput)
        {
            var branchOffice = _branchOfficesRepository.GetAll();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            if (!String.IsNullOrEmpty(searchInput.NameFilter))
                branchOffice = branchOffice.Where(c => c.Name.ToLower().Contains(searchInput.NameFilter)
                || c.Name.ToLower().Equals(searchInput.NameFilter));

            if (!String.IsNullOrEmpty(searchInput.CodeFilter))
                branchOffice = branchOffice.Where(c => c.Code == searchInput.CodeFilter);


            return branchOffice.MapTo<List<BranchOfficesDto>>().OrderBy(s => s.Code).ToList().ToPagedList(currentPageIndex, searchInput.MaxResultCount);

        }

        public void Update(UpdateBranchOfficesInput input)
        {
            var branchOffice = _branchOfficesRepository.Get(input.Id);
            if (branchOffice == null)
            {
                throw new UserFriendlyException("Could not found the Branch, may be it's deleted.");
            }

            branchOffice.Name = input.Name;
            branchOffice.Location = input.Location;
            branchOffice.Code = input.Code;

            _branchOfficesRepository.Update(branchOffice);
           
        }

        public void Update(BranchOfficesDto input)
        {
            var branchOffice = _branchOfficesRepository.Get(input.Id);
            if (branchOffice == null)
            {
                throw new UserFriendlyException("Could not found the Branch, may be it's deleted.");
            }

            branchOffice.Name = input.Name;
            branchOffice.Location = input.Location;
            branchOffice.Code = input.Code;

            _branchOfficesRepository.Update(branchOffice);
        }
        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
            {
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            }
            return users;
        }

     
    }
}
