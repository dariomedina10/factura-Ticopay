using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Sellers.Dto;

namespace TicoPay.Sellers
{
    public class SellerAppService : ApplicationService, ISellerAppService
    {
        private readonly IRepository<Seller, int> _sellerRepository;
        private readonly ISellerManager _sellerManager;

        public SellerAppService(IRepository<Seller, int> sellerRepository, ISellerManager sellerManager)
        {
            _sellerRepository = sellerRepository;
            _sellerManager = sellerManager;
        }

        public List<Seller> GetSellers()
        {
            try
            {
                var sellers = _sellerRepository.GetAllList().OrderBy(a => a.Name);
                return new List<Seller>(sellers.MapTo<List<Seller>>());
            }
            catch
            {
                return null;
            }

        }

        public Seller GetSeller(int id)
        {
            try
            {
                var seller = _sellerRepository.Get(id);
                return seller;
            }
            catch
            {
                return null;
            }
        }
    }
}
