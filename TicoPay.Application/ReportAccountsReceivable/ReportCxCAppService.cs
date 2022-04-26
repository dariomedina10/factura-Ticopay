using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Invoices;
using TicoPay.ReportAccountsReceivable.Dto;
using Abp.Domain.Repositories;
using TicoPay.Clients;
using TicoPay.Users;
using BCR;

namespace TicoPay.ReportAccountsReceivable
{
    public class ReportCxCAppService : ApplicationService, IReportCxCAppService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IUserAppService _userAppService;

        public ReportCxCAppService(IRepository<Invoice, Guid> invoiceRepository, IUserAppService userAppService, IInvoiceManager invoiceManager)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _userAppService = userAppService;
        }


        public IList<Invoice> SearchReportAccountsReceivable(ReportAccountsReceivableInputDto searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();
            //var query = "";
            var incoiceList = _invoiceRepository.GetAll().Where(x => drawers.Any(y => y.Value == x.DrawerId));

            //if (searchInput.Query == null)
            //    searchInput.Query = "";

            //if (searchInput.Query == "")
            //{
            //    query = searchInput.Query.ToLower();
            //    incoiceList = incoiceList.Where(a => a.Client.Name.ToLower().Contains(query) || a.Client.Name.ToLower().Equals(query) || a.Client.Code.Equals(query));
            //}

            if (searchInput.Status != null)
            {
                incoiceList = incoiceList.Where(a => a.Status == searchInput.Status.Value);
                if(searchInput.Status == Status.Parked)
                {
                    incoiceList = incoiceList.Where(i =>
                        !(i.Notes.Any(n => n.NoteType == NoteType.Credito) &&
                        ((i.Notes.Where(n => n.NoteType == NoteType.Credito).Sum(n => n.Total)) >= (i.InvoiceLines.Sum(l => l.LineTotal)))));
                }
            }

            if (searchInput.PaymetnMethodType != null)
            {
                incoiceList = incoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymetnMethodType == searchInput.PaymetnMethodType.Value);
                incoiceList = incoiceList.Where(a => a.Status == Status.Completed);
            }

            if (searchInput.ClientId != null)
                incoiceList = incoiceList.Where(a => a.ClientId == searchInput.ClientId.Value);

            if (searchInput.InitialDate != null)
                incoiceList = incoiceList.Where(a => a.DueDate >= searchInput.InitialDate.Value);

            if (searchInput.FinalDate != null)
                incoiceList = incoiceList.Where(a => a.DueDate <= searchInput.FinalDate.Value);

            if (searchInput.CodigoMoneda != null)
                incoiceList = incoiceList.Where(a => a.CodigoMoneda == searchInput.CodigoMoneda.Value);

            if (searchInput.GroupsId != null)
                incoiceList = incoiceList.Where(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));

            if (searchInput.ConsecutiveNumber != null)
            {
                var numero = searchInput.ConsecutiveNumber.Length < 10 ? searchInput.ConsecutiveNumber : searchInput.ConsecutiveNumber.Substring(10);
                var number = numero.ToString().PadLeft(10, '0');
                incoiceList = incoiceList.Where(a => a.ConsecutiveNumber.Substring(10) == number);
            }

            if (searchInput.TypeDocument != null)
                incoiceList = incoiceList.Where(a => a.TypeDocument == searchInput.TypeDocument.Value);

            if (searchInput.BranchOfficeId != null)
            {
                incoiceList = incoiceList.Where(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
            }

            if (searchInput.DrawerId != null)
            {
                incoiceList = incoiceList.Where(a => a.DrawerId == searchInput.DrawerId);
            }

            IList<Invoice> lista =incoiceList.Include(a => a.Client).Include(a => a.InvoiceLines).Include(a => a.Notes).Include(a => a.InvoicePaymentTypes).OrderBy(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate).ToList();
            InvoiceList(lista);
            return lista;
        }

        private void InvoiceList(IList<Invoice> lista)
        {
            RateOfDay rate = new RateOfDay();
            foreach(var l in lista)
            {
                if (l.ChangeType == 1)
                    l.ChangeType = rate.RateDate(l.DueDate).rate;
            }
        }


        public IList<Client> GetAllClientsList()
        {
            try
            {
                var list = _invoiceManager.GetAllListClientsSoftDelete();
                return list;
            }
            catch (Exception)
            {

                //int hola = 0;
            }
            return null;
        }
    }
}
