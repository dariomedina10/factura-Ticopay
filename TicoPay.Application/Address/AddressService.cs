using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.General;
using System.Data.Entity;
using Abp.Application.Services;
using Abp.Dependency;



namespace TicoPay.Address
{
    public class AddressService : ApplicationService, IAddressService, ITransientDependency
    {
        private readonly IRepository<Provincia, int> _provinceRepository;
        private readonly IRepository<Canton, int> _cantonRepository;
        private readonly IRepository<Distrito, int> _distritoRepository;
        private readonly IRepository<Barrio, int> _barrioRepository;
        private readonly IRepository<Country, int> _countryRepository;

        public AddressService(IRepository<Provincia, int> provinceRepository, IRepository<Canton, int> cantonRepository, IRepository<Distrito, int> distritoRepository, IRepository<Barrio, int> barrioRepository, IRepository<Country, int> countryRepository)
        {
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _cantonRepository = cantonRepository;
            _distritoRepository = distritoRepository;
            _barrioRepository = barrioRepository;
        }

        public IList<Country> GetAllCountries()
        {
            return _countryRepository.GetAll().ToList();
        }

        public IList<Provincia> GetAllProvincias()
        {
            return _provinceRepository.GetAll().ToList();
        }

       
        public IList<Canton> GetCantonesByProvinciaId(int? provinciaId)
        {
            var query = _cantonRepository.GetAll();
            if (provinciaId != null)
            {
                query = query.Where(c => c.ProvinciaID == provinciaId);
            }
            return query.OrderBy(c => c.NombreCanton).ToList();
        }

        public IList<Distrito> GetDistritosByCantonId(int? cantonId)
        {
            var query = _distritoRepository.GetAll();
            if (cantonId != null)
            {
                query = query.Where(d => d.CantonID == cantonId);
            }
            return query.OrderBy(d => d.NombreDistrito).ToList();
        }

        public IList<Barrio> GetBarriosByDistritoId(int? distritoId)
        {
            var query = _barrioRepository.GetAll();
            if (distritoId != null)
            {
                query = query.Where(b => b.DistritoID == distritoId);
            }
            return query.OrderBy(b => b.NombreBarrio).ToList();
        }

        public int GetDistritoIdByBarrioId(int barrioId)
        {
            var barrio = _barrioRepository.GetAll().Where(b => b.Id == barrioId).Include(b => b.Distrito).FirstOrDefault();
            if (barrio != null)
            {
                return barrio.DistritoID;
            }
            return 0;
        }

        public int GetCantonIdByDistritoId(int distritoId)
        {
            var distrito = _distritoRepository.GetAll().Where(d => d.Id == distritoId).Include(b => b.Canton).FirstOrDefault();
            if (distrito != null)
            {
                return distrito.CantonID;
            }
            return 0;
        }

        public int GetProvinciaIdByCantonId(int cantoId)
        {
            var canton = _cantonRepository.GetAll().Where(c => c.Id == cantoId).Include(b => b.Provincia).FirstOrDefault();
            if (canton != null)
            {
                return canton.ProvinciaID;
            }
            return 0;
        }

        public Country GetCountryById(int id)
        {
            return _countryRepository.Get(id);
        }
    }
}
