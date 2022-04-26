using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Sellers
{
    public interface ISellerManager : IDomainService
    {
        Seller Get(int id);

        void Create(Seller @seller);

        IList<Seller> ListSellers();
    }
}
