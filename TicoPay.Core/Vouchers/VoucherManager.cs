using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Vouchers
{
    public class VoucherManager : DomainService, IVoucherManager
    {
        private readonly IRepository<Voucher, Guid> _voucherRepository;

        public VoucherManager(IRepository<Voucher, Guid> voucherRepository)
        {
            _voucherRepository = voucherRepository;
           
        }

        public Voucher Create(Voucher voucher)
        {
            var newvoucher = _voucherRepository.Insert(voucher);

            if (newvoucher == null)
                throw new UserFriendlyException("Hubo un error creando el comprobante");

            return newvoucher;
        }

        public Voucher Update(Voucher voucher)
        {
            var result = _voucherRepository.Update(voucher);                  

            return result;
        }

        public Voucher Get(Guid id)
        {
            var voucher = _voucherRepository.FirstOrDefault(id);
                       
            if (voucher == null)
            {
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted!");
            }
            return voucher;
        }

        public List<Voucher> Getby(Expression<Func<Voucher,bool>> predicate)
        {
            var list = _voucherRepository.GetAll().Where(predicate);
            return list.ToList();
        }

        public IQueryable<Voucher> GetAll()
        {
            var list = _voucherRepository.GetAll();
            return list;
        }
    }
}
