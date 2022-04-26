using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.General;

namespace TicoPay.Address
{
    public interface IAddressService : IApplicationService
    {
        int GetDistritoIdByBarrioId(int barrioId);
        int GetCantonIdByDistritoId(int distritoId);
        int GetProvinciaIdByCantonId(int cantonId);
        IList<Canton> GetCantonesByProvinciaId(int? provinciaId);
        IList<Distrito> GetDistritosByCantonId(int? cantonId);
        IList<Barrio> GetBarriosByDistritoId(int? distritoId);
        IList<Provincia> GetAllProvincias();
        IList<Country> GetAllCountries();
        Country GetCountryById(int id);
    }
}
