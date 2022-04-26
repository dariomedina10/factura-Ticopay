using Abp.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Imports;
using TicoPay.Imports.Dto;

namespace TicoPay.Web.Controllers
{
    public class ImportsController : Controller
    {
        private readonly IImportsAppService _importsAppService;

        public ImportsController(IImportsAppService importsAppService)
        {
            _importsAppService = importsAppService;
        }

        [HttpPost]
        public JsonResult ImportClients(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        FileDto importClientDto = new FileDto();
                        importClientDto.FileStream = upload.InputStream;
                        var result = _importsAppService.ImportClient(importClientDto);
                        if (result.HasErrors)
                        {
                            var errors = result.ItemsWithErrors as List<string>;
                            return Json(new { Success = false, Error = string.Join(Environment.NewLine, errors.ToArray()) });
                        }
                        if (result.ImportedItemsCount == 0)
                        {
                            return Json(new { Success = false, Error = "Error al importar Clientes. El archivo dado no tiene el formato correcto." });
                        }
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false, Error = "Debe seleccionar un archivo .csv con la información de los clientes." });
        }

        [HttpPost]
        public JsonResult ImportService(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        FileDto importServiceDto = new FileDto();
                        importServiceDto.FileStream = upload.InputStream;
                        var result = _importsAppService.ImportService(importServiceDto);
                        if (result.HasErrors)
                        {
                            var errors = result.ItemsWithErrors as List<string>;
                            return Json(new { Success = false, Error = errors.ToArray() });
                        }
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false, Error = "Debe seleccionar un archivo .csv con la información de los servicios." });
        }

        [HttpPost]
        public JsonResult ImportProduct(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        FileDto importProductDto = new FileDto();
                        importProductDto.FileStream = upload.InputStream;
                        var result = _importsAppService.ImportProduct(importProductDto);
                        if (result.HasErrors)
                        {
                            var errors = result.ItemsWithErrors as List<string>;
                            return Json(new { Success = false, Error = errors.ToArray() });
                        }
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false, Error = "Debe seleccionar un archivo .csv con la información de los productos." });
        }

        [HttpPost]
        public JsonResult ImportBranchOffices(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        FileDto importBranchOfficeDto = new FileDto();
                        importBranchOfficeDto.FileStream = upload.InputStream;
                        var result = _importsAppService.ImportBranchOffices(importBranchOfficeDto);
                        if (result.HasErrors)
                        {
                            var errors = result.ItemsWithErrors as List<string>;
                            return Json(new { Success = false, Error = string.Join(Environment.NewLine, errors.ToArray()) });
                        }
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false, Error = "Debe seleccionar un archivo .csv con la información de las sucursales." });
        }

        [HttpPost]
        public JsonResult ImportDrawers(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        FileDto importDrawerseDto = new FileDto();
                        importDrawerseDto.FileStream = upload.InputStream;
                        var result = _importsAppService.ImportDrawers(importDrawerseDto);
                        if (result.HasErrors)
                        {
                            var errors = result.ItemsWithErrors as List<string>;
                            return Json(new { Success = false, Error = string.Join(Environment.NewLine, errors.ToArray()) });
                        }
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false, Error = "Debe seleccionar un archivo .csv con la información de las cajas." });
        }
    }
}