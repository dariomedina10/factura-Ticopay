using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Sellers
{
    public class SellerManager : DomainService, ISellerManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Seller, int> _sellerRepository;

        public SellerManager(IRepository<Seller, int> SellerRepository)
        {
            _sellerRepository = SellerRepository;
            EventBus = NullEventBus.Instance;
        }

        public void Create(Seller @Seller)
        {
            _sellerRepository.Insert(@Seller);
        }

        public Seller Get(int id)
        {
            var @Seller = _sellerRepository.FirstOrDefault(id);
            if (@Seller == null)
            {
                throw new UserFriendlyException("Could not found the Seller, maybe it's deleted!");
            }

            return @Seller;
        }

        public IList<Seller> ListSellers()
        {
            throw new NotImplementedException();
        }
    }
}
