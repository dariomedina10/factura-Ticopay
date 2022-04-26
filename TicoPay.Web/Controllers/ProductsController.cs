using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Inventory;
using TicoPay.Inventory.Dto;
using TicoPay.Common;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Abp.ObjectMapping;
using System.Data.Entity.Validation;
using Abp.Domain.Repositories;
using TicoPay.Taxes;
using static TicoPay.Inventory.Product;

namespace TicoPay.Web.Controllers
{
    public class ProductsController : TicoPayControllerBase
    {
        public readonly IInventoryAppServices _productAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<Tax, Guid> _taxRepository;

        public ProductsController(IInventoryAppServices productAppService , IObjectMapper objectMapper, IRepository<Tax, Guid> taxRepository)
        {
            _productAppService = productAppService;
            _objectMapper = objectMapper;
            _taxRepository = taxRepository;
        }


        // GET: Products
        public ActionResult Index(int? page)
        {
            SearchProductsInput model = new SearchProductsInput();
           try
            {
            
                model.Brands = _productAppService.GetBrands();
                model.Taxes = _productAppService.GetAllTaxes();
                model.Entities = _productAppService.SearchProducts(model);              
                model.Control = "Products";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Productos";
            }
            return View(model);
        }


        [HttpPost]
        public ViewResultBase Search(SearchProductsInput model)
        {
            try
            {
                var entities = _productAppService.SearchProducts(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Productos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _productAppService.GetDetail(id);
           entity.Users =_productAppService.GetAllUser();

            return PartialView("_detailsPartial", entity);
        }


        public ActionResult Create()
        {
            CreateProductInput viewModel = new CreateProductInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.Taxes = _productAppService.GetAllTaxes();
               
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateProductInput viewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var newVm = new CreateProductInput();
                    newVm.result = _productAppService.Create(viewModel);
                    
                   ModelState.Clear();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.Taxes = _productAppService.GetAllTaxes();
                    newVm.ErrorDescription = "¡Producto guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.Taxes = _productAppService.GetAllTaxes();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }

            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }

            return PartialView("_createPartial", viewModel);
        }


        [HttpPost]
        public ActionResult CreateAjax(CreateProductInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _productAppService.Create(viewModel);
                    ModelState.Clear();
                    return Json(result);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos:" + ModelState.ToErrorMessage();
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
            }
            viewModel.Taxes = _productAppService.GetAllTaxes();
            return PartialView("_createPartial", viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            UpdateProductInput viewModel = new UpdateProductInput();
            try
            {
                viewModel =  _productAppService.GetEdit(id);

                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            viewModel.Taxes = _productAppService.GetAllTaxes();
            return PartialView("_editPartial", viewModel);
        }


        [HttpPost]
        public ActionResult Edit(UpdateProductInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _productAppService.Update(viewModel);
                    viewModel.Taxes = _productAppService.GetAllTaxes();

                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Producto guardado exitosamente!";
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
            viewModel.Taxes = _productAppService.GetAllTaxes();

            return PartialView("_editPartial", viewModel);
        }


        public ActionResult AjaxPage(int? page, string name, Guid? brand, Guid? tax, Estatus? estado, decimal? priceSince, decimal? priceUntil)
        {
            SearchProductsInput model = new SearchProductsInput();
            model.Page = page;
            model.BrandFilter = brand;
            model.NameFilter = name;
            model.TaxFilter = tax;
            model.PriceSinceFilter = priceSince;
            model.PriceUntilFilter = priceUntil;

            try
            {
                var Entities = _productAppService.SearchProducts(model);
                model.Entities = Entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Productos";
            }
            return PartialView("_listPartial", model);
        }
        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _productAppService.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }


    }
}