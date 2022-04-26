using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abp.Runtime.Validation;
using Abp.ObjectMapping;
using System.Data.Entity.Validation;
using Abp.Domain.Repositories;
using TicoPay.Common;
using TicoPay.Drawers;
using TicoPay.Drawers.Dto;
using TicoPay.Users.Dto;
using TicoPay.MultiTenancy;
using LinqKit;
using TicoPay.Editions;
using Abp.Web.Models;

namespace TicoPay.Web.Controllers
{
    public class DrawersController : TicoPayControllerBase
    {
        private readonly IDrawersAppService _drawersAppService;    
        private readonly ITenantAppService _tenantAppService;


        public DrawersController(IDrawersAppService drawersAppService, ITenantAppService tenantAppService)
        {
            _drawersAppService = drawersAppService;           
            _tenantAppService = tenantAppService;
        }

        // GET: Drawers
        public ActionResult Index()
        {
            SearchDrawerInput model = new SearchDrawerInput();
            try
            {
                var tenant = _tenantAppService.GetById((int)AbpSession.TenantId);
                model.Edition = _tenantAppService.GetAllTicoPayEditions().Where(a => a.Id == tenant.EditionId).FirstOrDefault();
                model.MaxResultCount = 5;
                model.BranchOffices = _drawersAppService.GetBranchOffices().ToList();
                model.Entities = _drawersAppService.SearchDrawers(model);
                model.Control = "Drawers";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar Cajas";
            }
            return View(model);
        }

        [HttpPost]
        public ViewResultBase Search(SearchDrawerInput model)
        {
            try
            {
                model.MaxResultCount = 5;
                var entities = _drawersAppService.SearchDrawers(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar la Caja";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _drawersAppService.GetDetail(id);
            entity.Users = _drawersAppService.GetAllUser();

            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateDrawerInput viewModel = new CreateDrawerInput();
            viewModel.isEnabled = _tenantAppService.isDrawerEnabled(AbpSession.TenantId.GetValueOrDefault(), false);
            try
            {
                if (viewModel.isEnabled)
                {
                    viewModel.ErrorCode = ErrorCodeHelper.None;
                    viewModel.BranchOffice = _drawersAppService.GetBranchOffices().ToList();


                    viewModel.ErrorDescription = "";
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Ha alcanzado el limite de cajas disponible para su Plan.";
                }
                
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateDrawerInput viewModel)
        {
            try
            {
                viewModel.isEnabled = _tenantAppService.isDrawerEnabled(AbpSession.TenantId.GetValueOrDefault(), false);
                var newVm = new CreateDrawerInput();
                if (!viewModel.isEnabled)
                {
                    newVm.ErrorCode = ErrorCodeHelper.Error;
                    newVm.ErrorDescription = "Ha alcanzado el limite de cajas disponible para su Plan.";
                    return PartialView("_createPartial", newVm);
                }
                else if (ModelState.IsValid)
                {
                    
                    newVm.isEnabled = viewModel.isEnabled;
                    newVm.result = _drawersAppService.Create(viewModel);
                   // newVm.BranchOffice = _drawersAppService.GetBranchOffices().ToList();
                    ModelState.Clear();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.BranchOffice = _drawersAppService.GetBranchOffices().ToList();
                    newVm.ErrorDescription = "¡Caja guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }

            catch (Exception e)
            {
                
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            viewModel.BranchOffice =_drawersAppService.GetBranchOffices().ToList();
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult CreateAjax(CreateDrawerInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _drawersAppService.Create(viewModel);
                    ModelState.Clear();
                    return Json(result);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
            }
          
            return PartialView("_createPartial", viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            UpdateDrawerInput viewModel = new UpdateDrawerInput();
            try
            {
                viewModel = _drawersAppService.GetEdit(id);
                if (!viewModel.IsOpen)
                {
                    viewModel.ErrorCode = ErrorCodeHelper.None;
                    viewModel.ErrorDescription = "";
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "La caja se encuentra abierta, debe cerrar la caja para poder actualizarla";
                }
               
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            viewModel.BranchOffices = _drawersAppService.GetBranchOffices().ToList();

            return PartialView("_editPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(UpdateDrawerInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    _drawersAppService.Update(viewModel);
                    viewModel.BranchOffices = _drawersAppService.GetBranchOffices().ToList();
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Caja guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            //viewModel.Taxes = _productAppService.GetAllTaxes();
           
            return PartialView("_editPartial", viewModel);
        }

        public ActionResult AjaxPage(int? page ,string code)
        {
            SearchDrawerInput model = new SearchDrawerInput();
            model.Page = page;
            model.MaxResultCount = 5;
            model.CodeFilter = code;
            try
            {
                var Entities = _drawersAppService.SearchDrawers(model);
                model.Entities = Entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar Caja";
            }
            return PartialView("_listPartial", model);
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {

                _drawersAppService.Delete(id);
                return Json(new AjaxResponse { Success = true });
            }
            catch (Exception ex)
            {

                return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Message } });
            }
        }

        public ActionResult Open()
        {
            CreateDrawerInput viewModel = new CreateDrawerInput();
            try
            {
                var drawerUser = _drawersAppService.getUserDrawers(null);
                viewModel.ErrorCode = ErrorCodeHelper.None;                
                viewModel.BranchOffice = _drawersAppService.getUserbranch().ToList();
                viewModel.DrawerUser = (from d in drawerUser select new SelectListItem { Value= d.Drawer.Id.ToString(), Text=d.Drawer.Code }).ToList();
                if (viewModel.BranchOffice.Count > 0)
                {
                    var openDrawer = _drawersAppService.getUserDrawersOpen();
                    viewModel.IsOpen = true;
                    if (openDrawer != null)
                    {
                        viewModel.Id = openDrawer.Id;
                        viewModel.Description = openDrawer.Description;
                        viewModel.Code = openDrawer.Code;
                        viewModel.BranchOfficeId = openDrawer.BranchOfficeId;
                        viewModel.IsOpen = !openDrawer.IsOpen;
                        
                    }
                    if (viewModel.IsOpen)
                        ViewBag.Title = "Aperturar Caja:";
                    else
                        ViewBag.Title = "Cierre de Caja:";

                    viewModel.ErrorDescription = "";
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "El usuario no posee cajas asociadas";
                }
                
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;              
                viewModel.ErrorDescription = "Error al obtener datos de la caja.";
            }
            return PartialView("_openPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Open(CreateDrawerInput viewModel)
        {
            try
            {
                var drawerUser = _drawersAppService.getUserDrawers(null);
                viewModel.BranchOffice = _drawersAppService.getUserbranch().ToList();
                viewModel.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                if (ModelState.IsValid)
                {
                    if (viewModel.IsOpen)
                    {
                        _drawersAppService.OpenDrawer(viewModel.Id);
                        viewModel.ErrorDescription = "¡Caja abierta exitosamente!";
                        viewModel.IsOpen = false;
                    }
                    else
                    {
                        _drawersAppService.CloseDrawer(viewModel.Id);
                        viewModel.ErrorDescription = "¡Caja cerrada exitosamente!";
                        viewModel.IsOpen = true;
                    }                        

                    ModelState.Clear();
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;



                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = ModelState.ToErrorMessage();
                }
                
            }

            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            
            return PartialView("_openPartial", viewModel);
        }

        [ActionName("GetDrawer")]
        public ActionResult GetDrawer(Guid? id)
        {
            var drawer = _drawersAppService.getUserDrawers(id);

            var resp = drawer.Select(x => new SelectListItem()
            {
                Value = x.Drawer.Id.ToString(),
                Text = x.Drawer.Code
            }).ToList();
            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Caja" });

            return Json(resp, JsonRequestBehavior.AllowGet);

        }
    }
}