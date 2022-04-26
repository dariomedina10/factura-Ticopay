using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Imports.Dto;

namespace TicoPay.Imports
{
    public interface IImportsAppService : IApplicationService
    {
        ImportResult ImportClient(FileDto importClientDto);
        ImportResult ImportService(FileDto importServiceDto);
        ImportResult ImportProduct(FileDto importProductDto);
        ImportResult ImportBranchOffices(FileDto importBranchOfficeDto);
        ImportResult ImportDrawers(FileDto importDrawersDto);
    }
}
