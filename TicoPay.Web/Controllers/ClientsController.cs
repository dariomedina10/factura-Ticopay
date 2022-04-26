using Abp.Dependency;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Common;
using TicoPay.General;
using TicoPay.GroupConcept.Dto;
using TicoPay.Groups;
using TicoPay.Groups.Dto;
using TicoPay.MultiTenancy;
using TicoPay.MultiTenancy.Dto;
using TicoPay.Services.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Authorize]
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    public class ClientsController : TicoPayControllerBase
    {
        private readonly IClientAppService _clientAppClient;
        private readonly IGroupAppService _groupAppService;
        private readonly IIocResolver _iocResolver;
        private readonly ITenantAppService _tenantAppService;

        public ClientsController(IClientAppService clientAppClient, IGroupAppService groupAppService, IIocResolver iocResolver, ITenantAppService tenantAppService)
        {
            _clientAppClient = clientAppClient;
            _groupAppService = groupAppService;
            _iocResolver = iocResolver;
            _tenantAppService = tenantAppService;
        }

        [HttpPost]
        public ViewResultBase Search(SearchClientsInput model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entities = _clientAppClient.SearchClients(model);
                    model.Entities = entities;
                    //model.InvoicesList = _clientAppClient.GetAllInvoicesWithStatusIsParked();
                    model.Groups = GetGroupsSelectList();
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string code, string name, string identification, string email, string groupId)
        {
            SearchClientsInput model = new SearchClientsInput();
            try
            {
                if (code.Length <= 10 && name.Length <= 80 && identification.Length <= 20 && email.Length <= 100)
                {
                    model.CodeFilter = code;
                    model.NameFilter = name;
                    model.IdentificationFilter = identification;
                    model.EmailFilter = email;
                    model.GroupId = groupId;
                    model.Entities = _clientAppClient.SearchClients(model);
                    //model.InvoicesList = _clientAppClient.GetAllInvoicesWithStatusIsParked();
                    model.Groups = GetGroupsSelectList();
                    model.Control = "Clients";
                    model.Action = "Search";
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchClientsInput model = new SearchClientsInput();
            try
            {
                model.Query = "";
                model.CodeFilter = "";
                model.NameFilter = "";
                model.IdentificationFilter = "";
                model.EmailFilter = "";
                model.GroupId = "";
                model.Entities = _clientAppClient.SearchClients(model);
                //model.InvoicesList = _clientAppClient.GetAllInvoicesWithStatusIsParked();
                model.Groups = GetGroupsSelectList();
                model.Control = "Clients";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string code, string name, string identification, string email, string groupId, int? page)
        {
            SearchClientsInput model = new SearchClientsInput();
            model.Page = page;
            model.CodeFilter = code;
            model.NameFilter = name;
            model.IdentificationFilter = identification;
            model.EmailFilter = email;
            model.GroupId = groupId;
            try
            {
                if (ModelState.IsValid)
                {
                    model.Groups = GetGroupsSelectList();
                    model.Entities = _clientAppClient.SearchClients(model);
                    //model.InvoicesList = _clientAppClient.GetAllInvoicesWithStatusIsParked();
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }
            return PartialView("_listPartial", model);
        }


        public ActionResult Details(Guid id)
        {
            var entity = _clientAppClient.GetDetail(id);
            entity.Users = _clientAppClient.GetAllUser();

            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateClientInput viewModel = new CreateClientInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
                //viewModel.IdentificationType = Invoices.XSD.IdentificacionTypeTipo.NoAsiganda;
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            finally
            {
                LoadRelatedValues(viewModel);
            }
            return View("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateClientInput viewModel)
        {
            try
            {
                ModelState.Clear();

                ModelState.AddValidationErrors(viewModel, _iocResolver);

                if (ModelState.IsValid)
                {
                    _clientAppClient.Create(viewModel);

                    ModelState.Clear();

                    var newVm = new CreateClientInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Cliente guardado exitosamente!";
                    LoadRelatedValues(newVm, false);
                    return View("_createPartial", newVm);
                }
                else
                {
                    LoadRelatedValues(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                }
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
            }
            return View("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult CreateAjax(CreateClientInput viewModel, List<ServiceDto> listService, List<GroupConceptsDto> listGroupService)
        {
         
            try
            {
                if (listService != null)
                {
                    foreach (ServiceDto item in listService)
                    {
                        item.TaxId = new Guid();
                    }
                }
                viewModel.ClientServiceList = listService;
                viewModel.GroupsConceptsList = listGroupService;

                ModelState.Clear();                
                ModelState.AddValidationErrors(viewModel, _iocResolver);                

                if (ModelState.IsValid)
                {                    
                    _clientAppClient.Create(viewModel);

                   

                    ModelState.Clear();

                    var newVm = new CreateClientInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Cliente guardado exitosamente!";
                    LoadRelatedValues(newVm, false);
                    return PartialView("_createPartial", newVm);
                }
                else
                {
                    LoadRelatedValues(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                }
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
                return PartialView("_createPartial", viewModel);
            }
            catch (AbpValidationException abpValidationException)
            {
               foreach (ValidationResult validationResult in abpValidationException.ValidationErrors)
                {
                    viewModel.ErrorDescription = string.Concat(viewModel.ErrorDescription, validationResult.ErrorMessage);
                };
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription =e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValues(viewModel);
            }
            
            return PartialView("_createPartial", viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            UpdateClientInput viewModel = new UpdateClientInput();
            try
            {
                viewModel = _clientAppClient.GetEdit(id);
                viewModel.ClientServiceList = _clientAppClient.GetListServices(viewModel.Id);
                viewModel.GroupsConceptsList = _clientAppClient.GetListGroupsConcepts(viewModel.Id);
                viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                viewModel.CostoSms = GetCostoSmsActual(viewModel.Identification);
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            finally
            {
                //viewModel.GroupsSelect = _clientAppClient.GetAllGroups();
                //viewModel.GroupsSelected = _clientAppClient.ListGroupsTest(id);
                //viewModel.ServicesSelect = _clientAppClient.GetAllServices();
                //viewModel.ClientServiceList = _clientAppClient.ListServicesStrings(id);
                //viewModel.GroupsConceptsSelect = _clientAppClient.GetAllGroupsConcepts();
                //viewModel.GroupsConceptsSelected = _clientAppClient.ListGroupServicesStrings(id);
                //if (viewModel.BarrioId.HasValue)
                //{
                //    viewModel.DistritoID = _clientAppClient.GetDistritoByBarrios(viewModel.BarrioId.Value);
                //    viewModel.BarriosList = _clientAppClient.GetBarriosByDistrito(viewModel.DistritoID);
                //    viewModel.CantonID = _clientAppClient.GetCantonByDistrito(viewModel.DistritoID);
                //    viewModel.Distritos = _clientAppClient.GetDistritosByCanton(viewModel.CantonID);
                //    viewModel.Province = _clientAppClient.GetAllProvince();
                //    viewModel.ProvinciaID = _clientAppClient.GetIdProvinceByCanton(viewModel.CantonID);
                //    viewModel.Cantons = _clientAppClient.GetCantonByProvince(viewModel.ProvinciaID);
                //}
                LoadRelatedValuesEdit(viewModel);

            }
            return View("_editPartial", viewModel);
        }

        [ActionName("GetCanton")]
        public ActionResult GetCanton(int? id)
        {
            var canton = _clientAppClient.GetCantonByProvince(id);

            var resp = canton.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreCanton
            }).ToList();
            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Canton" });

            return Json(resp, JsonRequestBehavior.AllowGet);

        }

        [ActionName("GetDistritos")]
        public ActionResult GetDistritos(int? id)
        {
            var distrito = _clientAppClient.GetDistritosByCanton(id);

            var resp = distrito.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreDistrito
            }).ToList();

            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Distrito" });

            return Json(resp, JsonRequestBehavior.AllowGet);

        }

        [ActionName("GetBarrios")]
        public ActionResult GetBarrios(int? id)
        {
            var barrio = _clientAppClient.GetBarriosByDistrito(id);

            var resp = barrio.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreBarrio
            }).ToList();

            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Barrio" });

            return Json(resp, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit(UpdateClientInput viewModel, List<ServiceDto> listService, List<GroupConceptsDto> listGroupService)
        {
            try
            {
                ModelState.Clear();
                /* Se requiere rellenar el valor del TaxID cuando tiene servicios recurrentes para evitar errores de validación del modelo */
                if (listService != null)
                {
                    foreach (ServiceDto item in listService)
                    {
                        item.TaxId = new Guid();
                    }
                }
                viewModel.ClientServiceList = listService;
                viewModel.GroupsConceptsList = listGroupService;
       
                ModelState.AddValidationErrors(viewModel, _iocResolver);

                if (ModelState.IsValid)
                {
                    _clientAppClient.Update(viewModel);
                    SetCostoSmsActual(viewModel.Identification, viewModel.CostoSms);

                    LoadRelatedValuesEdit(viewModel);

                    viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                    viewModel.CostoSms = GetCostoSmsActual(viewModel.Identification);

                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Cliente guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }

                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                LoadRelatedValuesEdit(viewModel, false);
                viewModel.ErrorDescription = "Error en los datos.";
                return PartialView("_editPartial", viewModel);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValuesEdit(viewModel, false);

                if (viewModel.Birthday != null)
                {
                    viewModel.Month = viewModel.Birthday.Value.Month;
                    viewModel.Day = viewModel.Birthday.Value.Day;
                    viewModel.Year = viewModel.Birthday.Value.Year;
                }
                viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                return PartialView("_editPartial", viewModel);
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                if (ex.GetModelErrors().Any(d => d.Key != "Email" && d.Key != "ContactEmail"))
                {
                    viewModel.ErrorDescription = ex.GetModelErrorMessage();
                }
                else
                {
                    viewModel.ErrorDescription = ex.GetModelErrorMessage(false);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValuesEdit(viewModel, false);

                if (viewModel.Birthday != null)
                {
                    viewModel.Month = viewModel.Birthday.Value.Month;
                    viewModel.Day = viewModel.Birthday.Value.Day;
                    viewModel.Year = viewModel.Birthday.Value.Year;
                }
                viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                return PartialView("_editPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                LoadRelatedValuesEdit(viewModel, false);

                if (viewModel.Birthday != null)
                {
                    viewModel.Month = viewModel.Birthday.Value.Month;
                    viewModel.Day = viewModel.Birthday.Value.Day;
                    viewModel.Year = viewModel.Birthday.Value.Year;
                }
                viewModel.CanEditCostoSms = CanEditCostoSms(viewModel.Identification);
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _clientAppClient.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        private void LoadRelatedValues(CreateClientInput viewModel, bool useCurrentRecordValues = true)
        {
            viewModel.Groups = _clientAppClient.GetAllGroups();
            viewModel.Services = _clientAppClient.GetAllServices().Where(d => d.IsRecurrent).ToList();
            viewModel.GroupsConcepts = _clientAppClient.GetAllGroupsConcepts();
            viewModel.Province = _clientAppClient.GetAllProvince();

            viewModel.Cantons = viewModel.CantonID != 0 && useCurrentRecordValues ?
                _clientAppClient.GetCantonByProvince(viewModel.ProvinciaID) :
                new List<Canton> { new Canton() { NombreCanton = "Seleccione un Canton", Id = 0 } };

            viewModel.Distritos = viewModel.DistritoID != 0 && useCurrentRecordValues ?
                _clientAppClient.GetDistritosByCanton(viewModel.CantonID) :
                viewModel.Distritos = new List<Distrito> { new Distrito() { NombreDistrito = "Seleccione un Distrito", Id = 0 } };

            viewModel.BarriosList = viewModel.BarrioId != 0 && useCurrentRecordValues ?
                _clientAppClient.GetBarriosByDistrito(viewModel.DistritoID) :
                new List<Barrio> { new Barrio() { NombreBarrio = "Seleccione un Barrio", Id = 0 } };

            viewModel.Paises = _clientAppClient.GetAllCountries();
        }

        public SelectList GetGroupsSelectList()
        {
            SearchGroupsInput model = new SearchGroupsInput();
            return new SelectList(_groupAppService.SearchGroups(model).ToArray(), "Id", "Name");
        }

        private void LoadRelatedValuesEdit(UpdateClientInput viewModel, bool useCurrentRecordValues = true)
        {
            viewModel.GroupsSelect = _clientAppClient.GetAllGroups();
            viewModel.GroupsSelected = _clientAppClient.ListGroupsTest(viewModel.Id);           
            viewModel.ServicesSelect = _clientAppClient.GetAllServices().Where(d => d.IsRecurrent).ToList();
            viewModel.GroupsConceptsSelect = _clientAppClient.GetAllGroupsConcepts();
            //viewModel.ClientServiceList = _clientAppClient.GetListServices(viewModel.Id);
            //viewModel.GroupsConceptsList = _clientAppClient.GetListGroupsConcepts(viewModel.Id);
            viewModel.Paises = _clientAppClient.GetAllCountries();
            if (viewModel.BarrioId.GetValueOrDefault() > 0)
            {
                viewModel.DistritoID = _clientAppClient.GetDistritoByBarrios(viewModel.BarrioId.Value);
                viewModel.BarriosList = _clientAppClient.GetBarriosByDistrito(viewModel.DistritoID);
                viewModel.CantonID = _clientAppClient.GetCantonByDistrito(viewModel.DistritoID);
                viewModel.Distritos = _clientAppClient.GetDistritosByCanton(viewModel.CantonID);
                viewModel.Province = _clientAppClient.GetAllProvince();
                viewModel.ProvinciaID = _clientAppClient.GetIdProvinceByCanton(viewModel.CantonID);
                viewModel.Cantons = _clientAppClient.GetCantonByProvince(viewModel.ProvinciaID);
            }
            else
            {
                viewModel.Province = _clientAppClient.GetAllProvince();

                viewModel.Cantons = viewModel.CantonID != 0 && useCurrentRecordValues ?
                    _clientAppClient.GetCantonByProvince(viewModel.ProvinciaID) :
                    new List<Canton> { new Canton() { NombreCanton = "Seleccione un Canton", Id = 0 } };

                viewModel.Distritos = viewModel.DistritoID != 0 && useCurrentRecordValues ?
                    _clientAppClient.GetDistritosByCanton(viewModel.CantonID) :
                    viewModel.Distritos = new List<Distrito> { new Distrito() { NombreDistrito = "Seleccione un Distrito", Id = 0 } };

                viewModel.BarriosList = viewModel.BarrioId != 0 && useCurrentRecordValues ?
                    _clientAppClient.GetBarriosByDistrito(viewModel.DistritoID) :
                    new List<Barrio> { new Barrio() { NombreBarrio = "Seleccione un Barrio", Id = 0 } };
            }
        }

        private bool CanEditCostoSms(string clientIdentificationNumber)
        {
            bool canEdit = false;
            TenantDto tenantDto = _tenantAppService.GetBy(t => t.IdentificationNumber == clientIdentificationNumber);
            if (tenantDto != null)
            {
                var ticoPayTenant = _tenantAppService.GetBy(t => t.TenancyName == Migrations.SeedData.TicoPayTenantCreator.TicoPayTenantName);
                if (ticoPayTenant != null && ticoPayTenant.Id == CurrentUnitOfWork.GetTenantId())
                {
                    canEdit = true;
                }
            }
            return canEdit;
        }

        private decimal GetCostoSmsActual(string clientIdentificationNumber)
        {
            decimal costoSms = 0;
            TenantDto tenantDto = _tenantAppService.GetBy(t => t.IdentificationNumber == clientIdentificationNumber);
            if (tenantDto != null)
            {
                costoSms = tenantDto.CostoSms;
            }
            return costoSms;
        }

        private void SetCostoSmsActual(string clientIdentificationNumber, decimal costoSms)
        {
            TenantDto tenantDto = _tenantAppService.GetBy(t => t.IdentificationNumber == clientIdentificationNumber);
            if (tenantDto != null)
            {
                _tenantAppService.UpdateCostoSms(tenantDto.Id, costoSms);
            }
        }
    }
}