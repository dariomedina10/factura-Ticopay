using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.ReportAccountsReceivable.Dto;
using TicoPay.ReportClosing.Dto;
using TicoPay.Users;
using LinqKit;
using TicoPay.Invoices.XSD;
using BCR;
using TicoPay.MultiTenancy;
using TicoPay.Common;
using TicoPay.ReportStatusInvoices.Dto;
using TicoPay.Authorization.Roles;
using Abp.UI;

namespace TicoPay.ReportClosing
{
    public class ReportClosingAppService : ApplicationService, IReportClosingAppService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly IRepository<PaymentInvoice, Guid> _paymentRepository;
        private readonly IRepository<Payment, Guid> _truePaymentRepository;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IUserAppService _userAppService;
        RateOfDay _rateOfDay = new RateOfDay();
        private readonly TenantManager _tenantManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<User, long> _userRepository;

        public ReportClosingAppService(TenantManager tenantManager, IRepository<Invoice, Guid> invoiceRepository, IInvoiceManager invoiceManager,
            IUserAppService userAppService, IRepository<PaymentInvoice, Guid> paymentRepository, IRepository<Note, Guid> noteRepository, RoleManager roleManager, IRepository<User, long> userRepository, IRepository<Payment, Guid> truePaymentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _userAppService = userAppService;
            _paymentRepository = paymentRepository;
            _noteRepository = noteRepository;
            _tenantManager = tenantManager;
            _roleManager = roleManager;
            _userAppService = userAppService;
            _userRepository = userRepository;
            _truePaymentRepository = truePaymentRepository;
        }

        public IList<ReportClosingDto> SearchReportClosing(ReportClosingInputDto<ReportClosingDto> searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {

                var predicate = PredicateBuilder.New<Invoice>(true);

                predicate = predicate.And(x => drawers.Any(y => y.Value == x.DrawerId));

                if (searchInput.InitialDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.DueDate) >= searchInput.InitialDate);

                if (searchInput.FinalDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.DueDate) <= searchInput.FinalDate);

                if (searchInput.GroupsId != null)
                    predicate = predicate.And(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));

                if (searchInput.ConsecutiveNumber != null)
                {
                    var numero = searchInput.ConsecutiveNumber.Length < 10 ? searchInput.ConsecutiveNumber : searchInput.ConsecutiveNumber.Substring(10);
                    var number = numero.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);

                }
                if (searchInput.TypeDocument != null)
                    predicate = predicate.And(a => a.TypeDocument == searchInput.TypeDocument.Value);

                if (searchInput.CodigoMoneda != null)
                {
                    string value = searchInput.CodigoMoneda.ToString();
                    if (Enum.TryParse(value, out FacturaElectronicaResumenFacturaCodigoMoneda facturaMoneda))
                    {
                        predicate = predicate.And(a => a.CodigoMoneda == facturaMoneda);
                    }
                }

                if (searchInput.BranchOfficeId != null && searchInput.DrawerId == null)
                {
                    predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId.Value);
                }

                if (searchInput.DrawerId != null)
                {
                    predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId.Value);
                }

                if (searchInput.UserId != null)
                {
                    var usario = _userRepository.Get(Convert.ToInt16(searchInput.UserId));
                    predicate = predicate.And(a => a.UserName == usario.UserName);
                }

                if (searchInput.PaymentOrigin != null)
                {
                    {
                        List<Guid> byPayment = new List<Guid>();

                        var byPaymentSite = _truePaymentRepository.GetAll().Where(c => c.PaymentOrigin == searchInput.PaymentOrigin.Value);

                        foreach (Payment payment in byPaymentSite)
                        {
                            byPayment.Add(payment.PaymentInvoices.Select(c=>c.InvoiceId).FirstOrDefault());
                        }

                        predicate = predicate.And(c => byPayment.Any(a => a == c.Id));
                    }
                }

                var invoiceList = _invoiceRepository.GetAll().Include(a => a.InvoiceLines).Include(a => a.Notes).Include(a => a.Client).Include(a => a.InvoicePaymentTypes).Include(a=>a.Drawer).Where(predicate);

                // obterner la tasa 
                DateTime year = DateTimeZone.Now();
                DateTime Inicio = new DateTime(year.Year, 1, 1);
                var fechaInicio = searchInput.InitialDate == null ? Inicio : searchInput.InitialDate.Value;
                var fechaFin = searchInput.FinalDate == null ? DateTimeZone.Now() : searchInput.FinalDate.Value;
                List<ResultRateDto> rate = _rateOfDay.GetDayRate(fechaInicio, fechaFin);
                var tenant = _tenantManager.Get((int)AbpSession.TenantId);

                var query = (from invoice in invoiceList
                             orderby invoice.ConsecutiveNumber descending
                             select new ReportClosingDto
                             {
                                 InvoiceId = invoice.Id,

                                 ClientId = invoice.ClientId,
                                 ClientCode = invoice.Client != null ? invoice.Client.Code : 0,
                                 ClientName = invoice.Client != null ? invoice.Client.Name : string.Empty,
                                 LastName = invoice.Client != null ? invoice.Client.LastName : string.Empty,
                                 ConsecutiveNumber = invoice.ConsecutiveNumber,
                                 UserName = invoice.UserName,
                                 Debit = (invoice.Notes.Where(x => x.NoteType == NoteType.Debito).ToList().Count == 0 ? 0 : invoice.Notes.Where(x => x.NoteType == NoteType.Debito).Sum(x => x.Total)),
                                 Credit = (invoice.Notes.Where(x => x.NoteType == NoteType.Credito).ToList().Count == 0 ? 0 : invoice.Notes.Where(x => x.NoteType == NoteType.Credito).Sum(x => x.Total)),
                                 Notes = (from p in invoice.Notes
                                      orderby invoice.ConsecutiveNumber descending
                                          select new ReportStatusNoteDto
                                          {
                                              InvoiceId = p.InvoiceId,
                                              NoteType = p.NoteType,
                                              Amount = p.Amount,
                                              CodigoMoneda = p.CodigoMoneda,
                                              ConsecutiveNumber = p.ConsecutiveNumber,
                                              TaxAmount = p.TaxAmount,
                                              Total = p.Total,
                                              SendInvoice = p.SendInvoice,
                                              StatusTribunet = p.StatusTribunet,
                                              CreationTime = p.CreationTime,
                                              IsNoteReceptionConfirmed = p.IsNoteReceptionConfirmed,
                                          }).ToList(),
                                 InvoiceLines = (from l in invoice.InvoiceLines
                                             orderby invoice.ConsecutiveNumber descending
                                                 select new Invoices.Dto.InvoiceLineDto
                                                 {
                                                     InvoiceId = l.InvoiceId
                                                 }).ToList(),
                                 PaymentInvoices = (from p in invoice.InvoicePaymentTypes
                                                orderby invoice.ConsecutiveNumber descending
                                                select new ReportPaymentInvoice 
                                                    {
                                                        InvoiceId = p.InvoiceId,
                                                        Amount = p.Amount,
                                                        PaymentDate = p.Payment.PaymentDate,
                                                        LastModificationTime = p.LastModificationTime,
                                                        PaymentInvoiceType = p.Payment.PaymentType,
                                                        PaymetnMethodType = p.Payment.PaymetnMethodType,
                                                        Transaction = p.Payment.Transaction
                                                    }).ToList(),
                                 Number = invoice.Number,
                                 CreationTime = invoice.CreationTime,
                                 CodigoMoneda = invoice.CodigoMoneda,
                                 CodigoMonedaTenant = tenant.CodigoMoneda,
                                 CodigoMonedaFiltro = searchInput.CodigoMoneda,
                                 DueDate = invoice.DueDate,
                                 Status = invoice.Status,
                                 Total = invoice.Total,
                                 TotalC = invoice.Total,
                                 ShowConversion = tenant.IsConvertUSD,
                                 TasaConversion = 1,
                                 TypeDocument = invoice.TypeDocument

                             }).ToList();

                    QueryTotal(query, rate);

                return query;
            }
            else
            {
                return null;
            }
        }

        void QueryTotal(List<ReportClosingDto> query, List<ResultRateDto> rate)
        {
            var tenantId = AbpSession.TenantId;
            var tenant = _tenantManager.Get((int)AbpSession.TenantId);
            foreach (var list in query)
            {

                list.TotalC = CalculateInvoiceAmount2(list.Total, list.CodigoMoneda, tenant.CodigoMoneda, rate, list.DueDate);
                list.TasaConversion = GetResultRates(list.CreationTime, rate);
                list.InvoicePaymentTypes = new PaymetnMethodType();
                if (list.ShowConversion)
                {
                    list.TasaConversion = GetResultRates(list.CreationTime, rate);
                }
                else
                {
                    list.TasaConversion = 1;
                }
            };
        }
        public decimal CalculateInvoiceAmount2(decimal Total, FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMoneda, FacturaElectronicaResumenFacturaCodigoMoneda moneda, List<ResultRateDto> rate, DateTime CreateTime)
        {
            decimal facturado;
            Enum.TryParse(moneda.ToString(), out NoteCodigoMoneda Moneda);
            Enum.TryParse(CodigoMoneda.ToString(), out NoteCodigoMoneda codigoMoneda);
            facturado = CalcularMonto(Total, codigoMoneda, Moneda, rate, CreateTime);

            return facturado;
        }


        //public IList<Invoice> SearchReportClosing(ReportClosingInputDto searchInput)
        //{
        //    var query = "";
        //    if (searchInput.Query != null)
        //        query = searchInput.Query.ToLower();

        //    var invoiceList = _invoiceRepository.GetAll().Include(a => a.InvoicePaymentTypes);
        //    //incoiceList = incoiceList.Where(a => a.Client.Name.ToLower().Contains(query) || a.Client.Name.ToLower().Equals(query) || a.Client.Code.ToLower().Contains(query) || a.Client.Code.ToLower().Equals(query));

        //    invoiceList = invoiceList.Where(a => a.Status == Status.Completed);

        //    if (searchInput.InitialDate != null)
        //        invoiceList = invoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate >= searchInput.InitialDate.Value);

        //    if (searchInput.FinalDate != null)
        //        invoiceList = invoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate <= searchInput.FinalDate.Value);

        //    if (searchInput.GroupsId != null)
        //        invoiceList = invoiceList.Where(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));

        //    if (searchInput.CodigoMoneda != null)
        //        invoiceList = invoiceList.Where(a => a.CodigoMoneda == searchInput.CodigoMoneda.Value);

        //    if (searchInput.ConsecutiveNumber != null)
        //    {
        //        var numero = searchInput.ConsecutiveNumber.Substring(10);
        //        var number = numero.ToString().PadLeft(10, '0');
        //        invoiceList = invoiceList.Where(a => a.ConsecutiveNumber.Substring(10) == number);
        //    }
        //    if (searchInput.TypeDocument != null)
        //        invoiceList = invoiceList.Where(a => a.TypeDocument == searchInput.TypeDocument.Value);

        //    return invoiceList.Include(a => a.Client).Include(a => a.InvoiceLines).Include(a => a.Notes).OrderBy(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate).ToList();
        //}

        public IList<Invoice> SearchInvoices(ReportAccountsReceivableInputDto searchInput)
        {

            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();
            if (drawers != null)
            {
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
                    incoiceList = incoiceList.Where(a => a.Status == searchInput.Status.Value);

                if (searchInput.PaymetnMethodType != null)
                {
                    incoiceList = incoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymetnMethodType == searchInput.PaymetnMethodType.Value);
                    incoiceList = incoiceList.Where(a => a.Status == Status.Completed);
                }

                if (searchInput.ClientId != null)
                    incoiceList = incoiceList.Where(a => a.ClientId == searchInput.ClientId.Value);

                if (searchInput.InitialDate != null)
                    incoiceList = incoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate >= searchInput.InitialDate.Value);

                if (searchInput.FinalDate != null)
                    incoiceList = incoiceList.Where(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate <= searchInput.FinalDate.Value);

            if (searchInput.TypeDocument != null)
                incoiceList = incoiceList.Where(a => a.TypeDocument == searchInput.TypeDocument.Value);
                      
                return incoiceList.Include(a => a.Client).Include(a => a.InvoiceLines).Include(a => a.Notes).Include(a => a.InvoicePaymentTypes).OrderBy(a => a.InvoicePaymentTypes.FirstOrDefault().Payment.PaymentDate).ToList();
            }
            else
                return null;
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
            }
            return null;
        }


        public int GetCountClientsActive()
        {
            try
            {
                var countClient = _invoiceManager.GetAllListClientsSoftDelete().Count();
                return countClient;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public int GetCountUsersActive()
        {
            try
            {
                var countUser = _userAppService.GetAllListUsersSoftDelete().Count();
                return countUser;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public IList<ResultTotalDto> GetInvoices(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId)
        {
            var drawers = (from c in _userAppService.getUserDrawers(BranchOfficeId, DrawerId) select c.DrawerId).ToList();

            List<ResultRateDto> rate = _rateOfDay.GetDayRate(fechai, fechaf);

            var invoice = _invoiceRepository.GetAll().Include(a => a.Notes).Include(a => a.CodigoMoneda).Where(a => a.DueDate <= fechaf && a.DueDate >= fechai)
                .Where(x => drawers.Any(y => y.Value == x.DrawerId))
                .Select(cl => new
                {
                    Id = cl.Id,
                    CodigoMoneda = cl.CodigoMoneda,
                    Total = (decimal)cl.Total,
                    DueDate = cl.DueDate,
                    Debit = (cl.Notes.Where(x => x.NoteType == NoteType.Debito).ToList().Count == 0 ? 0 : cl.Notes.Where(x => x.NoteType == NoteType.Debito).Sum(x => x.Total)),
                    Credit = (cl.Notes.Where(x => x.NoteType == NoteType.Credito).ToList().Count == 0 ? 0 : cl.Notes.Where(x => x.NoteType == NoteType.Credito).Sum(x => x.Total))
                }).ToList();

            List<ResultTotalDto> invoicesLines = invoice
                       .GroupBy(l => l.DueDate.Month)
                       .Select(cl => new ResultTotalDto
                       {
                           Month = cl.FirstOrDefault().DueDate.Month.ToString(),
                           Quantity = cl.Count(),
                           Total = cl.Sum(c => CalculateInvoiceAmount(c.Total,c.Debit,c.Credit,c.CodigoMoneda,moneda,rate,c.DueDate)),
                       }).ToList();

            return invoicesLines;
        }

        public IList<ResultTotalDto> GetNoteCredit(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId)
        {
            var drawers = (from c in _userAppService.getUserDrawers(BranchOfficeId, DrawerId) select c.DrawerId).ToList();

            var note = _noteRepository.GetAll().Where(a => a.CreationTime <= fechaf && a.CreationTime >= fechai && a.NoteType == NoteType.Credito && a.IsDeleted == false)
                .Where(x => drawers.Any(y => y.Value == x.DrawerId))
                .Select(cl => new
                {
                    Id = cl.Id,
                    CreationTime = cl.CreationTime,
                    Total = cl.Total,
                    CodigoMoneda = cl.CodigoMoneda,
                }).ToList();

            List<ResultRateDto> rate = _rateOfDay.GetDayRate(fechai, fechaf);

            List<ResultTotalDto> notes = note
                       .GroupBy(l => l.CreationTime.Month)
                       .Select(cl => new ResultTotalDto
                       {
                           Month = cl.FirstOrDefault().CreationTime.Month.ToString(),
                           Quantity = cl.Count(),
                           Total = cl.Sum(c => CalcularMontoNotas(c.Total, c.CodigoMoneda, moneda, rate,c.CreationTime)),
                       }).ToList();

            return notes;
        }

        public IList<ResultTotalDto> GetNoteDebit(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId)
        {
            var drawers = (from c in _userAppService.getUserDrawers(BranchOfficeId, DrawerId) select c.DrawerId).ToList();

            var note = _noteRepository.GetAll().Where(a => a.CreationTime <= fechaf && a.CreationTime >= fechai && a.NoteType == NoteType.Debito && a.IsDeleted == false)
                .Where(x => drawers.Any(y => y.Value == x.DrawerId))
                .Select(cl => new
                {
                    Id = cl.Id,
                    CreationTime = cl.CreationTime,
                    Total = cl.Total,
                    CodigoMoneda = cl.CodigoMoneda,
                }).ToList();

            List<ResultRateDto> rate = _rateOfDay.GetDayRate(fechai, fechaf);

            List <ResultTotalDto> notes = note
                       .GroupBy(l => l.CreationTime.Month)
                       .Select(cl => new ResultTotalDto
                       {
                           Month = cl.FirstOrDefault().CreationTime.Month.ToString(),
                           Quantity = cl.Count(),
                           Total = cl.Sum(c => CalcularMontoNotas(c.Total, c.CodigoMoneda, moneda, rate, c.CreationTime)),
                       }).ToList();

            return notes;
        }

        public IList<ResultTotalDto> GetPayments(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId)
        {
            var drawers = (from c in _userAppService.getUserDrawers(BranchOfficeId, DrawerId) select c.DrawerId).ToList();

            var predicate = PredicateBuilder.New<Invoice>(true);
            predicate = predicate.And(a => a.DueDate >= fechai);
            predicate = predicate.And(a => a.DueDate <= fechaf);
            predicate = predicate.And(a => a.Status == Status.Completed);
            predicate = predicate.And(a => drawers.Any(y => y.Value == a.DrawerId));

            var invoice = _invoiceRepository.GetAll().Include(a => a.Notes).Where(predicate)
              .Select(cl => new
              {
                  Id = cl.Id,
                  CodigoMoneda = cl.CodigoMoneda,
                  Total = cl.Total,
                  DueDate = cl.DueDate,
                  Debit = (cl.Notes.Where(x => x.NoteType == NoteType.Debito).ToList().Count == 0 ? 0 : cl.Notes.Where(x => x.NoteType == NoteType.Debito).Sum(x => x.Total)),
                  Credit = (cl.Notes.Where(x => x.NoteType == NoteType.Credito).ToList().Count == 0 ? 0 : cl.Notes.Where(x => x.NoteType == NoteType.Credito).Sum(x => x.Total))
              });

            var paymentsline = _paymentRepository.GetAll().Where(a => a.Payment.PaymentDate <= fechaf && a.Payment.PaymentDate >= fechai);

            var invoicesLines = (from i in invoice
                                 join p in paymentsline on i.Id equals p.InvoiceId into g
                                 select new
                                 {
                                     ID = i.Id,
                                     CodigoMoneda = i.CodigoMoneda,
                                     Total = i.Total,
                                     Debit = i.Debit,
                                     Credit = i.Credit,
                                     DueDate = i.DueDate,
                                     Month = g.Count() > 0 ? g.Min(x => x.Payment.PaymentDate.Month) : 0
                                 }
                ).ToList();

            List<ResultRateDto> rate = _rateOfDay.GetDayRate(fechai, fechaf);

            var result = invoicesLines.GroupBy(l => l.Month)
                   .Select(cl => new ResultTotalDto
                   {
                       Month = cl.FirstOrDefault().Month.ToString(),
                       Quantity = cl.Count(),
                       Total = cl.Sum(c => CalculateInvoiceAmount(c.Total, c.Debit, c.Credit, c.CodigoMoneda, moneda, rate, c.DueDate))
                   }).ToList();

            return result;

            //List<ResultTotalDto> payments = _paymentRepository.GetAll().Where(a => a.PaymentDate <= fechaf && a.PaymentDate >= fechai)
            //.GroupBy(l => l.PaymentDate.Month)
            //.Select(cl => new ResultTotalDto
            //{
            //    Month = cl.FirstOrDefault().PaymentDate.Month.ToString(),
            //    Quantity = cl.Count(),
            //    Total = cl.Sum(c => c.Amount),
            //}).ToList();

            //return payments;
        }

        private decimal Suma(decimal valor1,decimal valor2)
        {
            return valor1 + valor2;
        }

        public bool GetUSDOfCRC(FacturaElectronicaResumenFacturaCodigoMoneda moneda)
        {
            if (moneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.USD))
                return false;
            return true;
        }


        private decimal GetResultRates(DateTime fechaf, List<ResultRateDto> rate)
        {
            decimal valor = 1;
            foreach (var result in rate)
            {
                if (fechaf.Date.Equals(result.Fecha.Date))
                {
                    valor = result.rate;

                    break;
                }
                
            }

            return valor;
        }

        private decimal CalcularMontoNotas(decimal monto, NoteCodigoMoneda CodigoMoneda, FacturaElectronicaResumenFacturaCodigoMoneda moneda, List<ResultRateDto> rate, DateTime CreateTime)
        {
            decimal valor = 0;
            if (Enum.TryParse(moneda.ToString(), out NoteCodigoMoneda Moneda))
            {
                valor = CalcularMonto(monto,CodigoMoneda,Moneda,rate,CreateTime);
            }
            return valor;
        }

        private decimal CalculateInvoiceAmount(decimal Total, decimal Debit, decimal Credit, FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda, FacturaElectronicaResumenFacturaCodigoMoneda moneda, List<ResultRateDto> rate, DateTime CreateTime)
        {
            decimal facturado,debito,credito;
            Enum.TryParse(moneda.ToString(), out NoteCodigoMoneda Moneda);
            Enum.TryParse(CodigoMoneda.ToString(), out NoteCodigoMoneda codigoMoneda);
            facturado = CalcularMonto(Total, codigoMoneda, Moneda, rate, CreateTime);
            debito = CalcularMonto(Debit, codigoMoneda, Moneda, rate, CreateTime);
            credito = CalcularMonto(Credit, codigoMoneda, Moneda, rate, CreateTime);
            return facturado + debito-credito;
        }

        private decimal CalcularMonto(decimal monto, NoteCodigoMoneda CodigoMoneda, NoteCodigoMoneda moneda, List<ResultRateDto> rate, DateTime CreateTime)
        {
            decimal taxes = GetResultRates(CreateTime, rate);
            decimal valor = 0;
            if (taxes > 0)
            {
            if (moneda.Equals(NoteCodigoMoneda.USD))
            {

                if (CodigoMoneda.Equals(NoteCodigoMoneda.CRC))
                {
                    valor = monto / taxes;
                }
                else
                {
                    valor = monto;
                }

            }
            else if (moneda.Equals(NoteCodigoMoneda.CRC))
            {
                if (CodigoMoneda.Equals(NoteCodigoMoneda.USD))
                {
                    valor = monto * taxes;
                }
                else
                {
                    valor = monto;
                }
                } 
            }

            return valor;
        }

        public IList<User> GetUserByRole()
        {
            var idTenant = AbpSession.TenantId.GetValueOrDefault();

            Role role = _roleManager.FindSuperAdminRole(idTenant);

            //Role role = _roleManager.FindByName("");
            if (role == null)
            {
                throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            }
            var user = _userRepository.GetAll().Include(u => u.Roles).Where(u => u.Roles.Any(r => r.RoleId != role.Id)).ToList();
            if (user == null)
            {
                throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }

            return user;
        }
    }
}
