using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.EntityFramework;
using TicoPay.Invoices;

namespace TicoPay.Migrations.SeedData
{
    public class AddListBanks
    {
        private readonly TicoPayDbContext _context;

        public const string BancoNacionalCostaRica = "BANCO NACIONAL DE COSTA RICA";
        public const string BancodeCostaRica = "BANCO DE COSTA RICA";
        public const string BancoBacSanJose = "BANCO BAC SAN JOSE SA";
        public const string BancoBCT = "BANCO BCT S.A.";
        public const string BancoCathayCostaRica = "BANCO CATHAY DE COSTA RICA, S.A.";
        public const string BancoCentralCostaRica = "BANCO CENTRAL DE COSTA RICA";
        public const string BancoCitiBankCostaRica = "BANCO CITIBANK DE COSTA RICA S.A.";
        public const string BancoGeneralCostaRica = "BANCO GENERAL (COSTA RICA), S.A.";
        public const string BancoHSBCCostaRica = "BANCO HSBC (COSTA RICA) S.A.";
        public const string BancoImprosa = "BANCO IMPROSA, S.A.";
        public const string BancoLafise = "BANCO LAFISE S.A.";
        public const string BancoPromerica = "BANCO PROMERICA";
        public const string BancoScotiabankCostaRica = "SCOTIABANK DE COSTA RICA, S.A.";
        public const string BancoPopularCostaRica = "BANCO POPULAR";


        public AddListBanks(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var tenants = _context.Tenants.Where(r => r.IsActive && !r.IsDeleted).Select(h => h.Id).ToList();


            foreach (var Id in tenants)
            {
                var banksToAddList = new List<Bank>()
            {
                //new Bank(){ Name = AddListBanks.BancoNacionalCostaRica, ShortName = "BNCR" },
                //new Bank(){ Name = AddListBanks.BancodeCostaRica, ShortName = "BCRI" },
                //new Bank(){ Name = AddListBanks.BancoBacSanJose, ShortName = "BSNJ" },
                //new Bank(){ Name = AddListBanks.BancoBCT, ShortName = "CCIO" },
                //new Bank(){ Name = AddListBanks.BancoCathayCostaRica, ShortName = "KTAY" },
                //new Bank(){ Name = AddListBanks.BancoCentralCostaRica, ShortName = "BCCR" },
                //new Bank(){ Name = AddListBanks.BancoCitiBankCostaRica, ShortName = "BACU" },
                //new Bank(){ Name = AddListBanks.BancoGeneralCostaRica, ShortName = "BAGE" },
                //new Bank(){ Name = AddListBanks.BancoHSBCCostaRica, ShortName = "BXBA" },
                //new Bank(){ Name = AddListBanks.BancoImprosa, ShortName = "BIMP" },
                //new Bank(){ Name = AddListBanks.BancoLafise, ShortName = "BCCE" },
                //new Bank(){ Name = AddListBanks.BancoPromerica, ShortName = "PRMK" },
                //new Bank(){ Name = AddListBanks.BancoScotiabankCostaRica, ShortName = "NOSC" },
                new Bank(){ Name = AddListBanks.BancoPopularCostaRica, ShortName = "BPCR" },
            };
                foreach (var bank in banksToAddList)
                {
                    bool addBank = _context.Banks.Count(d => d.TenantId == Id && d.IsActive && d.Name == bank.Name) == 0;
                    if (addBank)
                    {
                        CreateBank(bank, Id);
                    }
                }
            }

        }

        public void CreateBank(Bank BankPaymentInvoice, int tenantId)
        {
            BankPaymentInvoice.TenantId = tenantId;
            BankPaymentInvoice.IsActive = true;
  
            _context.Banks.Add(BankPaymentInvoice);
            _context.SaveChanges();
        }

    }

    public class BankComparer : IEqualityComparer<Bank>
    {
        public bool Equals(Bank x, Bank y)
        {
            if (x == null || y == null)
                return false;

            return (x.Name == y.Name && x.ShortName == y.ShortName);
        }
        public int GetHashCode(Bank obj)
        {
            return obj.GetHashCode();
        }
    }

}