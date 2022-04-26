using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Vouchers
{
    public interface IVoucherManager : IDomainService
    {
        Voucher Create(Voucher voucher);

        Voucher Get(Guid id);

        List<Voucher> Getby(Expression<Func<Voucher, bool>> predicate);

        IQueryable<Voucher> GetAll();

        Voucher Update(Voucher voucher);
    }
}
