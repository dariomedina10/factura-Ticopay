using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Sellers.Dto;

namespace TicoPay.Sellers
{
    public interface ISellerAppService : IApplicationService
    {
        List<Seller> GetSellers();
        Seller GetSeller(int id);
    }
}
