using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Application.Helpers;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Imports.Dto;
using TicoPay.Services;
using TicoPay.Services.Dto;
using TicoPay.Inventory;
using TicoPay.Inventory.Dto;
using TicoPay.Taxes;
using Abp.UI;
using TicoPay.BranchOffices;
using TicoPay.BranchOffices.Dto;
using TicoPay.Drawers;
using TicoPay.Drawers.Dto;

namespace TicoPay.Imports
{
    public class ImportsAppService : ApplicationService, IImportsAppService
    {
        private const char ColumnSeparator = ';';
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClientAppService _clientAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IInventoryAppServices _productAppService;
        private readonly ITaxAppService _taxAppService;
        private readonly IBranchOfficesAppService _branchOfficesAppService;
        private readonly IDrawersAppService _drawersAppService;

        public ImportsAppService(IDrawersAppService drawersAppService, IBranchOfficesAppService branchOfficesAppService, IInventoryAppServices productAppService, IUnitOfWorkManager unitOfWorkManager, IClientAppService clientAppService, IServiceAppService serviceAppService, ITaxAppService taxAppService)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _clientAppService = clientAppService;
            _serviceAppService = serviceAppService;
            _productAppService = productAppService; 
            _taxAppService = taxAppService;
            _branchOfficesAppService = branchOfficesAppService;
            _drawersAppService = drawersAppService;
        }

        public ImportResult ImportClient(FileDto importClientDto)
        {
            ImportResult importResult = new ImportResult();

            List<string> linesWithErrors = new List<string>();
            List<ClientDto> list = ReadLines<ClientDto>(importClientDto.FileStream, ClientImportLineValidator, out linesWithErrors);
            if (list != null)
            {
                foreach (var client in list)
                {
                    try
                    {
                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            _clientAppService.Create(client);
                            _unitOfWorkManager.Current.SaveChanges();

                            unitOfWork.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        linesWithErrors.Add($"{client.Name}: {ex.GetBaseException().Message}");
                    }
                }
                importResult.ImportedItemsCount = list.Count;
            }
            importResult.ItemsWithErrors = linesWithErrors;
            importResult.HasErrors = linesWithErrors.Count > 0;
            return importResult;
        }

        public ImportResult ImportService(FileDto importServiceDto)
        {
            ImportResult importResult = new ImportResult();

            List<string> linesWithErrors = new List<string>();
            List<ImportServiceDto> list = ReadLines<ImportServiceDto>(importServiceDto.FileStream, ServiceImportLineValidator, out linesWithErrors);
            if (list != null)
            {
                int line = 1;
                foreach (var service in list)
                {
                    try
                    {
                        bool canBeCreated = true;
                        ServiceDto serviceDto = ObjectMapper.Map<ServiceDto>(service);
                        serviceDto.Quantity = 1;
                        Tax tax = _taxAppService.GetBy(t => t.Name == service.TaxName);
                        if (tax != null)
                        {
                            serviceDto.TaxId = tax.Id;
                        }
                        else
                        {
                            linesWithErrors.Add($"No existe el impuesto {service.TaxName} linea número: " + line);
                            canBeCreated = false;
                        }
                        if (_serviceAppService.GetBy(s => s.Name == service.Name) != null && canBeCreated)
                        {
                            linesWithErrors.Add($"Ya existe un servicio con el nombre {service.Name} linea número: " + line);
                            canBeCreated = false;
                        }
                        if ((string.IsNullOrEmpty(service.Name)) && canBeCreated)
                        {
                            linesWithErrors.Add($"No debe contener espacion en blanco linea número: " + line);
                            canBeCreated = false;
                        }
                        if (canBeCreated)
                        {
                            _serviceAppService.Create(serviceDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        linesWithErrors.Add($"{service.Name}: {ex.GetBaseException().Message}");
                    }
                    line++;
                }
                importResult.ImportedItemsCount = list.Count;
            }
            importResult.ItemsWithErrors = linesWithErrors;
            importResult.HasErrors = linesWithErrors.Count > 0;
            return importResult;
        }

        public ImportResult ImportProduct(FileDto importProductDto)
        {
            Stream archivoCsv = importProductDto.FileStream;
            ImportResult importResult = new ImportResult();
            List<string> linesWithErrors = new List<string>();
            List<ImportProductDto> list = ReadLines<ImportProductDto>(archivoCsv, ProductImportLineValidator, out linesWithErrors);
            bool allerror = false;

            if (list != null && list.Count > 0)
            {
                int line = 1;
                foreach (var product in list)
                {
                    try
                    {
                        bool canBeCreated = true;
                        ProductDto productDto = ObjectMapper.Map<ProductDto>(product);
                        Tax tax = _taxAppService.GetBy(t => t.Name == product.TaxName);
                        if (tax != null)
                        {
                            productDto.TaxId = tax.Id;
                        }
                        else
                        {
                            linesWithErrors.Add($"No existe el impuesto asociado al producto {product.Name}. \n ");
                            canBeCreated = false;
                        }

                        if(list.Count == linesWithErrors.Count)
                        {
                            linesWithErrors.Clear();
                            allerror = true;
                        }

                        if (canBeCreated)
                        {
                            _productAppService.Create(productDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        linesWithErrors.Add($"{product.Name}: {ex.GetBaseException().Message}");
                    }
                    line++;
                }
                
                importResult.ImportedItemsCount = list.Count;
            }
            if (list.Count == 0 || allerror == true)
            {
                linesWithErrors.Add($"Error al importar productos, el archivo no tiene el formato esperado.");
            }

            importResult.ItemsWithErrors = linesWithErrors;
            importResult.HasErrors = linesWithErrors.Count > 0;
            return importResult;
        }

        public ImportResult ImportBranchOffices(FileDto importBranchOfficeDto)
        {
            Stream archivoCsv = importBranchOfficeDto.FileStream;
            ImportResult importResult = new ImportResult();
            List<string> linesWithErrors = new List<string>();
            List<ImportBranchOfficesDto> list = ReadLines<ImportBranchOfficesDto>(archivoCsv, BranchOfficeImportLineValidator, out linesWithErrors);
            bool allerror = false;

            if (list != null && list.Count > 0)
            {
                int line = 1;
                foreach (var branchOffices in list)
                {
                    try
                    {
                        bool canBeCreated = true;
                        BranchOfficesDto branchOfficeDto = ObjectMapper.Map<BranchOfficesDto>(branchOffices);
                       
                        if (list.Count == linesWithErrors.Count)
                        {
                            linesWithErrors.Clear();
                            allerror = true;
                        }

                        if (canBeCreated)
                        {
                            _branchOfficesAppService.Create(branchOfficeDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        linesWithErrors.Add($"{branchOffices.Name}: {ex.GetBaseException().Message}");
                    }
                    line++;
                }

                importResult.ImportedItemsCount = list.Count;
            }
            if (list.Count == 0 || allerror == true)
            {
                linesWithErrors.Add($"Error al importar sucursales, el archivo no tiene el formato esperado.");
            }

            importResult.ItemsWithErrors = linesWithErrors;
            importResult.HasErrors = linesWithErrors.Count > 0;
            return importResult;
        }

        public ImportResult ImportDrawers(FileDto importDrawersDto)
        {
            Stream archivoCsv = importDrawersDto.FileStream;
            ImportResult importResult = new ImportResult();
            List<string> linesWithErrors = new List<string>();
            List<ImportDrawersDto> list = ReadLines<ImportDrawersDto>(archivoCsv, DrawersImportLineValidator, out linesWithErrors);
            bool allerror = false;

            if (list != null && list.Count > 0)
            {
                int line = 1;
                foreach (var drawers in list)
                {
                    try
                    {
                        bool canBeCreated = true;
                        DrawerDto drawerDto = ObjectMapper.Map<DrawerDto>(drawers);

                        if (list.Count == linesWithErrors.Count)
                        {
                            linesWithErrors.Clear();
                            allerror = true;
                        }

                        if (canBeCreated)
                        {
                            _drawersAppService.Create(drawerDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        linesWithErrors.Add($"{drawers.Description}: {ex.GetBaseException().Message}");
                    }
                    line++;
                }

                importResult.ImportedItemsCount = list.Count;
            }
            if (list.Count == 0 || allerror == true)
            {
                linesWithErrors.Add($"Error al importar cajas, el archivo no tiene el formato esperado.");
            }

            importResult.ItemsWithErrors = linesWithErrors;
            importResult.HasErrors = linesWithErrors.Count > 0;
            return importResult;
        }

        private string ClientImportLineValidator(string[] columns, string[] values)
        {
            List<string> errors = new List<string>();
            if (columns == null || values == null || values.Length == 0 || columns.Length == 0 || columns.Length != values.Length)
            {
                errors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
            }

            if (errors.Count > 0)
            {
                return string.Join("* ", errors.ToArray());
            }
            return string.Empty;
        }

        private string ServiceImportLineValidator(string[] columns, string[] values)
        {
            List<string> errors = new List<string>();
            if (columns == null || values == null || values.Length == 0 || columns.Length == 0 || columns.Length != values.Length)
            {
                errors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
            }

            if (errors.Count > 0)
            {
                return string.Join("* ", errors.ToArray());
            }
            return string.Empty;
        }

        private string ProductImportLineValidator(string[] columns, string[] values)
        {
            List<string> errors = new List<string>();
            if (columns == null || values == null || values.Length == 0 || columns.Length == 0 || columns.Length != values.Length)
            {
                errors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
            }

            if (errors.Count > 0)
            {
                return string.Join("* ", errors.ToArray());
            }
            return string.Empty;
        }
        private string BranchOfficeImportLineValidator(string[] columns, string[] values)
        {
            List<string> errors = new List<string>();
            if (columns == null || values == null || values.Length == 0 || columns.Length == 0 || columns.Length != values.Length)
            {
                errors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
            }

            if (errors.Count > 0)
            {
                return string.Join("* ", errors.ToArray());
            }
            return string.Empty;
        }
        private string DrawersImportLineValidator(string[] columns, string[] values)
        {
            List<string> errors = new List<string>();
            if (columns == null || values == null || values.Length == 0 || columns.Length == 0 || columns.Length != values.Length)
            {
                errors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
            }

            if (errors.Count > 0)
            {
                return string.Join("* ", errors.ToArray());
            }
            return string.Empty;
        }

        private List<T> ReadLines<T>(Stream stream, Func<string[], string[], string> lineValidator, out List<string> linesWithErrors)
        {
            linesWithErrors = new List<string>();
            List<T> result = new List<T>();
            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string[] columnsNames = ReadLineValues(reader, ColumnSeparator);

                    while (!reader.EndOfStream)
                    {
                        string[] columnsValues = ReadLineValues(reader, ColumnSeparator);
                        if (!columnsValues.Any(d => d != string.Empty))
                            continue;
                        string errors = lineValidator.Invoke(columnsNames, columnsValues);
                        if (string.IsNullOrWhiteSpace(errors))
                        {
                            T instance = BuildObject<T>(columnsNames, columnsValues);
                            if (instance != null)
                            {
                                result.Add(instance);
                            }
                            else
                            {
                                linesWithErrors.Add("El archivo dado no tiene el formato esperado. Por favor verifique.");
                            }
                        }
                        else
                        {
                            linesWithErrors.Add(string.Join(ColumnSeparator.ToString(), columnsValues) + " | ERRORES: " + errors);
                        }
                    }
                }
            }
            return result;
        }

        private T BuildObject<T>(string[] columnsNames, string[] columnsValues)
        {
            T instance = default(T);
            if (columnsNames != null && columnsValues != null && columnsNames.Length == columnsValues.Length)
            {
                instance = Activator.CreateInstance<T>();
                for (int i = 0; i < columnsNames.Length; i++)
                {
                    string columnName = columnsNames[i];
                    string columnValue = columnsValues[i];
                    var property = typeof(T).GetProperty(columnName);
                    if (property != null)
                    {
                        var value = ParsePropertyType(property, columnValue);
                        if (value != null)
                        {
                            property.SetValue(instance, value);
                        }
                    }
                }
            }
            return instance;
        }

        private object ParsePropertyType(PropertyInfo property, string columnValue)
        {
            object value = null;
            try
            {
                if (property.PropertyType == typeof(decimal))
                {
                    decimal number = default(decimal);
                    decimal.TryParse(columnValue, out number);
                    value = number;
                }
                else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
                {
                    double number = default(double);
                    double.TryParse(columnValue, out number);
                    value = number;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    bool boolean = default(bool);
                    if (!bool.TryParse(columnValue, out boolean))
                    {
                        boolean = (string.Equals("si", columnValue.ToLower()) || string.Equals("s", columnValue.ToLower()));
                    }
                    value = boolean;
                }
                else if (property.PropertyType.IsEnum)
                {
                    int intValue = EnumHelper.Parse(property.PropertyType, columnValue);
                    value = intValue < 0 ? 0 : intValue;
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime = new DateTime();
                    DateTime.TryParse(columnValue, out dateTime);
                    value = dateTime;
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    Guid guid = new Guid();
                    Guid.TryParse(columnValue, out guid);
                    value = guid;
                }
                else if (property.PropertyType == typeof(string))
                {
                    value = columnValue;
                }
            }
            catch (Exception)
            {
            }
            return value;
        }

        private string[] ReadLineValues(StreamReader streamReader, char columnSeparator)
        {
            List<string> columns = new List<string>();
            string line = streamReader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                columns = line.Split(columnSeparator).ToList();
            }
            return columns.ToArray();
        }
    }
}
