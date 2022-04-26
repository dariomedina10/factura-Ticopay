using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using Quartz;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Common;

namespace TicoPay.Services
{
    public class ClientService : Event, IMustHaveTenant, IFullAudited
    {
        public virtual Service Service { get; protected set; }
        public Guid ServiceId { get; protected set; }

        public virtual Client Client { get; protected set; }
        public Guid ClientId { get; protected set; }

        public string Code { get; protected set; }

        [DataType(DataType.Date, ErrorMessage = "* El Formato de la fecha debe ser dd/MM/yyyy")]
        public DateTime InitDate { get; set; } // Fecha en que el worker o el servicio enpieza a trabajar

        public bool AllowLatePayment { get; protected set; }

        public DateTime? WorkerFirstEjecutionDate { get; set; }

        public DateTime? WorkerLastEjecutionDate { get; set; }

        public DateTime? WorkerNextEjecutionDate { get; set; }

        public int? LastGeneratedInvoice { get; set; }

        /// <summary>Gets or sets the Invoices. </summary>
        public virtual ICollection<Invoice> Invoices { get; protected set; }

        public ClientServiceState State { get; protected set; }

        public bool GeneratingInvoice { get; protected set; }

        public int TenantId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public bool CantCreateInvoice(string cronExp, bool isFirstInvoiceInTicopay)
        {
            var now = DateTimeZone.Now();

            var tempCron = new CronExpression(cronExp);
            var nextExecutionDate = tempCron.GetNextValidTimeAfter((WorkerNextEjecutionDate==null)?InitDate:now).Value.LocalDateTime;

            if (InitDate > now)
                return false;

            if (WorkerNextEjecutionDate == null)
            {
                if (!isFirstInvoiceInTicopay)
                {
                WorkerNextEjecutionDate = new DateTime(nextExecutionDate.Year, nextExecutionDate.Month, nextExecutionDate.Day, 
                    nextExecutionDate.Hour, nextExecutionDate.Minute, nextExecutionDate.Second);
                }
                if (now >= nextExecutionDate)
                    return true;
            }

            if (now < WorkerNextEjecutionDate)
                return false;

            return true;
        }

        public void SetNewEjecutionDates(string cronExp)
        {
            var tempCron = new CronExpression(cronExp);

            var nextExecutionDate = tempCron.GetNextValidTimeAfter(DateTimeZone.Now()).Value.LocalDateTime;

            if (WorkerFirstEjecutionDate == null)
                WorkerFirstEjecutionDate = DateTimeZone.Now();

            WorkerLastEjecutionDate = DateTimeZone.Now();

            WorkerNextEjecutionDate = new DateTime(nextExecutionDate.Year, nextExecutionDate.Month, nextExecutionDate.Day,
                nextExecutionDate.Hour, nextExecutionDate.Minute, nextExecutionDate.Second);
        }

        public void SetNextMonthNewEjecutionDate()
        {
            var nextExecutionDate = DateTimeZone.Now().AddMonths(1);

            if (WorkerFirstEjecutionDate == null)
                WorkerFirstEjecutionDate = DateTimeZone.Now();

            WorkerLastEjecutionDate = DateTimeZone.Now();

            WorkerNextEjecutionDate = new DateTime(nextExecutionDate.Year, nextExecutionDate.Month, 1, 0, 0, 0);
        }

        public void SetNextYearNewEjecutionDate()
        {
            var nextExecutionDate = DateTimeZone.Now().AddYears(1);

            if (WorkerFirstEjecutionDate == null)
                WorkerFirstEjecutionDate = DateTimeZone.Now();

            WorkerLastEjecutionDate = DateTimeZone.Now();

            WorkerNextEjecutionDate = new DateTime(nextExecutionDate.Year, nextExecutionDate.Month, 1, 0, 0, 0);
        }

        public void KillAdjusmentEjecution()
        {
            if (WorkerFirstEjecutionDate == null)
                WorkerFirstEjecutionDate = DateTimeZone.Now();

            WorkerLastEjecutionDate = DateTimeZone.Now();

            IsDeleted = true;
        }

        public decimal Quantity { get; set; } = 1;

        public decimal DiscountPercentage { get; set; }=0;


        public static ClientService Create(Service service, Client client, string cronExpresion, DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage, ClientServiceState clientServiceState = ClientServiceState.Active)
        {
#warning todo recordar reparar esto ver para que se puede utilizar el DateEvent
           
            var @cs = new ClientService
            {

                Id = new Guid(),
                Client = client,
                Service = service,
                Code = "",
                CronExpression = cronExpresion,
                State = clientServiceState,
                GeneratingInvoice = generateInvoice,
                AllowLatePayment = allowLatePayment,
                InitDate = initDateTime,
                DateEvent = initDateTime,
                WorkerFirstEjecutionDate = initDateTime,
                Quantity=quantity,
                DiscountPercentage=discountPercentage
                
            };
            if (cs.State == ClientServiceState.Adjustment)
            {
                cs.WorkerNextEjecutionDate = initDateTime;
            }
            return @cs;
        }

        protected ClientService()
        {

        }

        public void SetServiceId(Guid id)
        {
            ServiceId = id;
        }

        public void SetClientId(Guid id)
        {
            ClientId = id;
        }
    }

    public enum ClientServiceState
    {
        Active,
        Suspended,
        Adjustment
    }
}
