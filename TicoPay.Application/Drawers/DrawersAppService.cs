using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.Runtime.Session;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Drawers.Dto;
using TicoPay.Users;
using TicoPay.Invoices;
using Abp.Domain.Uow;
using TicoPay.BranchOffices;
using System.Data.Entity;
using TicoPay.BranchOffices.Dto;

namespace TicoPay.Drawers
{
    public class DrawersAppService : ApplicationService, IDrawersAppService
    {
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<Register, Guid> _registerRepository;
        private readonly IRepository<BranchOffice, Guid> _branchOfficerepository;

        public DrawersAppService(
            IRepository<Drawer, Guid> drawerRepository,
            UserManager userManager,
            IRepository<Register, Guid> registerRepository,
            IRepository<BranchOffice,Guid>  branchOfficerepository, IRepository<User, long> userRepository
            )
        {
            _drawerRepository = drawerRepository;
            _userManager = userManager;
            _registerRepository = registerRepository;
            _branchOfficerepository = branchOfficerepository;
            _userRepository = userRepository;
        }

        public DrawerDto Create(DrawerDto input)
        {
            var tenant = AbpSession.GetTenantId();
            input.CodeBranchOffice = DataCode(input.CodeBranchOffice, 1);
            input.Code = DataCode(input.Code, 2);

            var checkBranch = _branchOfficerepository.GetAll().Where(x => x.Code == input.CodeBranchOffice && x.TenantId == tenant).FirstOrDefault();

            if (checkBranch == null)
            {
                throw new UserFriendlyException("No existe la sucursal relacionada con la caja. \n");
            }

            BranchOffice branchOffice = _branchOfficerepository.FirstOrDefault(a => a.Code == input.CodeBranchOffice && a.TenantId == tenant);
            Guid branchId = branchOffice.Id;
            Drawer draw = Drawer.Create(AbpSession.GetTenantId(), input.Code, input.Description, branchId);
            
            var checkDraw = _drawerRepository.GetAll().Where(x => x.Code == input.Code && x.TenantId == tenant && x.BranchOfficeId == branchId).FirstOrDefault();
            var checkName = _drawerRepository.GetAll().Where(x => x.Description == input.Description && x.TenantId == tenant && x.BranchOfficeId == branchId).FirstOrDefault();

            if (checkDraw != null)
            {
                throw new UserFriendlyException("Existe una caja con el mismo número de Código en esa Sucursal. \n");
            }

            if (checkName != null)
            {
                throw new UserFriendlyException("Existe una caja con el mismo nombre en esa Sucursal. \n");
            }

            _drawerRepository.Insert(draw);

            Register register = Register.Create(AbpSession.GetTenantId(), input.Description, input.Code, 1, 0, 1, 0, 1, 0, 1, 0,
             "", "", true, false, false, true, true, false, draw.Id);

            _registerRepository.Insert(register);
            

            return draw.MapTo<DrawerDto>();
            
        }
        //formatea el codigo de sucursal o caja 
        public string DataCode(string Code, int opcion)
        {
            string newCode = "";
            if(opcion == 1)
            { 
            newCode = Code.PadLeft(3, '0');
            }
            if (opcion == 2)
            {
                newCode = Code.PadLeft(5, '0');
            }
            return newCode;
        }

        public DrawerDto Create(CreateDrawerInput input)
        {
            input.TenantId = AbpSession.GetTenantId();
            //Requerimiento Eliminado de la Tarjeta verificacion en el register
            //var register = _registerRepository.GetAll()
            //                                  .Where(x => x.TenantId == input.TenantId  && x.RegisterCode == input.Code)
            //                                  .FirstOrDefault();
            // if (register == null)
            //{
            var drawer = _drawerRepository.GetAll()
                                          .Where(x => x.TenantId == input.TenantId && x.Code == input.Code)
                                          .FirstOrDefault();
            if (drawer == null)
            {
                return CreateDrawer(input);
            }
            else
            {
                throw new UserFriendlyException("Ya existe una Caja Con el mismo Codigo en Esta Sucursal o se Encuentra Inactiva");
               
            }

           
        }

        [UnitOfWork]
        private DrawerDto CreateDrawer(CreateDrawerInput input)
        {
            //Procedimiento Eliminado en la tarjeta

            var drawer = Drawer.Create(input.TenantId, input.Code, input.Description, input.BranchOfficeId);

            _drawerRepository.Insert(drawer);

            Register register = Register.Create(AbpSession.GetTenantId(), input.Description,  input.Code, 1, 0, 1, 0, 1, 0, 1, 0,
             "", "", true, false, false, true, true, false, drawer.Id);
                        
            _registerRepository.Insert(register);

            return drawer.MapTo<DrawerDto>();
        }

        [UnitOfWork]
        public void Delete(Guid input)
        {
            var drawer = _drawerRepository.Get(input);
            if (drawer == null)
            {
                throw new UserFriendlyException("La caja no existe.");
            }
            if (!drawer.IsOpen)
            {

                var register = _registerRepository.GetAll().Where(x => x.DrawerId == input).FirstOrDefault();
                if ((register.LastInvoiceNumber==0)&& (register.LastNoteCreditNumber == 0) && (register.LastNoteDebitNumber == 0) &&
                    (register.LastTicketNumber == 0) && (register.LastVoucherNumber == 0))
                {
                    if (register != null)
                    {

                        register.IsDeleted = true;
                        _registerRepository.Update(register);

                    }
                    drawer.IsDeleted = true;
                    _drawerRepository.Update(drawer);
                }else
                {
                    throw new UserFriendlyException("La caja no puede ser eliminada pues posee documentos asociados.");
                }
                

            }else
                throw new UserFriendlyException("La caja no puede ser eliminada pues se encuentra abierta.");





        }

        public DrawerDto Get(Guid input)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
            {
                throw new UserFriendlyException("No se encuentrar usuario en el sistema.");
            }
            return users;
        }

        public DrawerDetailOutput GetDetail(Guid input)
        {
            var drawer = _drawerRepository.GetAll()
                                           .Where(a => a.Id == input).FirstOrDefault();
            if (drawer == null)
            {
                throw new UserFriendlyException("La caja no existe.");
            }
            return drawer.MapTo<DrawerDetailOutput>();
        }

        public UpdateDrawerInput GetEdit(Guid input)
        {
            var drawer = _drawerRepository.GetAll()
                                            .Where(a => a.Id == input).FirstOrDefault();            

            if (drawer == null)
            {
                throw new UserFriendlyException("La caja no existe.");
            }
            var drawerEdit = drawer.MapTo<UpdateDrawerInput>();

            var register = _registerRepository.GetAll().Where(x => x.DrawerId == drawer.Id).FirstOrDefault();
            if (register != null)
            {
                drawerEdit.RegisterID = register.Id;
                drawerEdit.LastInvoiceNumber = register.LastInvoiceNumber;
                drawerEdit.LastNoteCreditNumber = register.LastNoteCreditNumber;
                drawerEdit.LastNoteDebitNumber = register.LastNoteDebitNumber;
                drawerEdit.LastTicketNumber = register.LastTicketNumber;
                drawerEdit.LastVoucherNumber = register.LastVoucherNumber;
            }
            

            return drawerEdit;
        }

        //public ListResultDto<DrawerDto> GetDrawers()
        //{
        //    throw new NotImplementedException();
        //}

        public IPagedList<DrawerDto> SearchDrawers(SearchDrawerInput searchInput)
        {
            var drawer = _drawerRepository.GetAll();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            if (!String.IsNullOrEmpty(searchInput.CodeFilter))
                drawer = drawer.Where(x => x.Code == searchInput.CodeFilter);

            if (searchInput.BranchOfficeFilter != Guid.Empty)
                drawer = drawer.Where(x => x.BranchOfficeId == searchInput.BranchOfficeFilter);

            drawer = drawer.OrderBy(x => x.Code);

            return drawer.MapTo<List<DrawerDto>>().ToList().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public ListResultDto<DrawerDto> SearchDrawersApi(SearchDrawerInput searchInput)
        {
            var drawer = _drawerRepository.GetAll();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            if (!String.IsNullOrEmpty(searchInput.CodeFilter))
                drawer = drawer.Where(x => x.Code == searchInput.CodeFilter);

            if (searchInput.BranchOfficeFilter != Guid.Empty)
                drawer = drawer.Where(x => x.BranchOfficeId == searchInput.BranchOfficeFilter);

            drawer = drawer.OrderBy(x => x.Code);
            var drawerList = drawer.ToList();            

            return new ListResultDto<DrawerDto>(drawerList.MapTo<List<DrawerDto>>());
        }

        public IList<Drawer> SearchDrawersbyBranch(Guid Id)
        {
            var drawer = _drawerRepository.GetAll().Include("BranchOffice").Where(x => x.BranchOfficeId == Id);           

            return drawer.ToList();
        }

        public IList<Drawer> GetDrawers(SearchDrawerInput searchInput)
        {
            var drawer = _drawerRepository.GetAll();

            if (searchInput.TenantId>0)
                drawer = drawer.Where(x=>x.TenantId== searchInput.TenantId);

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            if (!String.IsNullOrEmpty(searchInput.CodeFilter))
                drawer = drawer.Where(x => x.Code == searchInput.CodeFilter);

            if (searchInput.BranchOfficeFilter != Guid.Empty)
                drawer = drawer.Where(x => x.BranchOfficeId == searchInput.BranchOfficeFilter);

            return drawer.ToList();
        }

        public void Update(DrawerDto input)
        {
            throw new NotImplementedException();
        }

        [UnitOfWork]
        public void Update(UpdateDrawerInput input)
        {
            var drawer = _drawerRepository.Get(input.Id);
            if (drawer == null)
            {
                throw new UserFriendlyException("La caja no existe.");
            }

            drawer.Code = input.Code;
            drawer.Description = input.Description;
            drawer.BranchOfficeId = input.BranchOfficeId;
            drawer.IsOpen = input.IsOpen;
            drawer.UserIdOpen = input.UserIdOpen;
            drawer.LastUserIdOpen = input.LastUserIdOpen;
            drawer.OpenUserDate = input.OpenUserDate;
            drawer.CloseUserDate = input.CloseUserDate;


            _drawerRepository.Update(drawer);

            var register = _registerRepository.Get(input.RegisterID);
            register.LastNoteDebitNumber = input.LastNoteDebitNumber;
            register.LastNoteCreditNumber = input.LastNoteCreditNumber;
            register.LastInvoiceNumber = input.LastInvoiceNumber;
            register.LastVoucherNumber = input.LastVoucherNumber;
            register.LastTicketNumber = input.LastTicketNumber;
            _registerRepository.Update(register);
        }

        public IList<BranchOffice> GetBranchOffices()
        {
            var branchOffices = _branchOfficerepository.GetAll()
                                                    .Where(x => x.TenantId == AbpSession.TenantId).ToList();
            if (branchOffices == null)
            {
                throw new UserFriendlyException("Could not found the branch");
            }
            return branchOffices;
        }

        public bool IsDrawersOpen (Guid Id)
        {
            //bool isOpen = false;

            var drawer = _drawerRepository.Get(Id);
            if (drawer == null)
            {
                throw new UserFriendlyException("La caja no existe.");
            }
            

            return drawer.IsOpen;
        }

        public Drawer getUserDrawersOpen()
        {
            var IdUser = AbpSession.GetUserId();
            if (IdUser == 0)
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }
            var drawer = _drawerRepository.GetAll().Include("BranchOffice").Where(x => x.UserIdOpen == IdUser && x.IsOpen==true).FirstOrDefault();
            return drawer;
        }

        public IList<DrawerUser> getUserDrawers(Guid? IdBranch)
        {
            var IdUser = AbpSession.GetUserId();
            if (IdUser == 0)
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }
            var user = _userRepository.Get(IdUser);

            var lista = user.DrawerUsers.Where(x=>x.Drawer!=null).AsEnumerable();

            if (IdBranch != null)
                lista = lista.Where(x => x.Drawer.BranchOfficeId == (Guid)IdBranch);

            return lista.ToList();
        }

        public IList<BranchOffice> getUserbranch()
        {
            var IdUser = AbpSession.GetUserId();
            if (IdUser == 0)
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }
            var user = _userRepository.Get(IdUser);

            var lista = user.DrawerUsers.AsEnumerable();

            var branchs = (from c in lista where c.Drawer!=null select c.Drawer.BranchOffice).Distinct().ToList();

            return branchs;
        }

        public void OpenDrawer (Guid Id)
        {
            var IdUser = AbpSession.GetUserId();
            if (IdUser > 0)
            {
                var d = getUserDrawersOpen();

                if (d != null)
                {
                    throw new UserFriendlyException("El usuario posee la caja " + d.Code + " abierta, debe realizar el cierre de la misma antes de abrir una nueva caja");
                }

                var drawer = _drawerRepository.Get(Id);
                if (drawer == null)
                {
                    throw new UserFriendlyException("Referencia de caja no encontrada, por favor verifique!.");
                }
                if (drawer.IsOpen)
                {
                    throw new UserFriendlyException("La caja ya se encuentra abierta por otro usuario.");
                }

                drawer.IsOpen = true;
                drawer.UserIdOpen = IdUser;
                drawer.OpenUserDate = DateTime.Now;
                _drawerRepository.Update(drawer);
            }
            else
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }


        }

        public void CloseDrawer(Guid Id)
        {
            var IdUser = AbpSession.GetUserId();
           if (IdUser > 0)
            {
                var drawer = _drawerRepository.Get(Id);
                if (drawer == null)
                {
                    throw new UserFriendlyException("Referencia de caja no encontrada, por favor verifique!.");
                }
                if (!drawer.IsOpen)
                {
                    throw new UserFriendlyException("La caja ya se encuentra cerrada.");
                }

                drawer.IsOpen = false;
                drawer.UserIdOpen = IdUser;
                drawer.CloseUserDate = DateTime.Now;
                _drawerRepository.Update(drawer);
            }         
            else
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }
        }
    }
}
