using System;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Quartz;
using Quartz.Impl;
using TicoPay.Invoices;
using TicoPay.Services;
using TicoPay.Clients;
using TicoPay.GroupConcept;

namespace WorkerInvoiceCreator
{
    public class WorkerProcess : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IGroupConceptsAppService _groupConcetsAppService;

        public WorkerProcess(AbpTimer timer, IInvoiceAppService invoiceAppService, IServiceAppService serviceAppService, IGroupConceptsAppService groupConcetsAppService) : base(timer)
        {
            _invoiceAppService = invoiceAppService;
            _serviceAppService = serviceAppService;
            _groupConcetsAppService = groupConcetsAppService;
        }

        [UnitOfWork]
        public virtual void Process()
        {
            //Get a scheduler, start the schedular before triggers or anything else
            IScheduler sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.Start();

            ScheduleCreateServiceInvoices(sched);
            //ScheduleCreateGroupConceptsInvoices(sched);
        }

        private void ScheduleCreateServiceInvoices(IScheduler sched)
        {
            var services = _serviceAppService.GetServicesEntities();
            int count = 1;

            foreach (var service in services)
            {
                //Create job
                IJobDetail job = JobBuilder.Create<ServiceInvoiceCreator>()
                        .WithIdentity(service.Name, "Create Service Invoices")
                        .Build();

                job.JobDataMap["pService"] = service;

                ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("Trigger" + count, "GroupServicesInvoices")
                        .WithCronSchedule(service.CronExpression)
                        .Build();

                // Schedule the job using the job and trigger 
                sched.ScheduleJob(job, trigger);
                count++;
            }
        }

        //private void ScheduleCreateGroupConceptsInvoices(IScheduler sched)
        //{
        //    var groupConcepts = _groupConcetsAppService.GetGroupConceptsEntities();
        //    int count = 1;

        //    foreach (var group in groupConcepts)
        //    {
        //        IJobDetail job = JobBuilder.Create<GroupConceptsInvoiceCreator>()
        //                .WithIdentity(group.Name, "Create Group Concepts Invoices")
        //                .Build();

        //        job.JobDataMap["pGroupConcepts"] = group;

        //        ITrigger trigger = TriggerBuilder.Create()
        //                .WithIdentity("Trigger" + count, "GroupConceptsInvoices")
        //                .WithCronSchedule(group.CronExpression)
        //                .Build();

        //        sched.ScheduleJob(job, trigger);
        //        count++;
        //    }
        //}

        public void ServiceInvoiceGeneration(Service service)
        {
            _invoiceAppService.CreateAllServiceInvoices(service);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected override void DoWork()
        {
            throw new NotImplementedException();
        }
    }

    [DisallowConcurrentExecution]
    public class ServiceInvoiceCreator : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IInvoiceAppService>().ImplementedBy<InvoiceAppService>().LifestyleTransient());
            var invoiceCreator = container.Resolve<IInvoiceAppService>();
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            Service pService = (Service)dataMap["pService"];

            invoiceCreator.CreateAllServiceInvoices(pService);
        }
    }

    //[DisallowConcurrentExecution]
    //public class GroupConceptsInvoiceCreator : IJob
    //{
    //    void IJob.Execute(IJobExecutionContext context)
    //    {
    //        var container = new WindsorContainer();
    //        container.Register(Component.For<IInvoiceAppService>().ImplementedBy<InvoiceAppService>().LifestyleTransient());
    //        var invoiceCreator = container.Resolve<IInvoiceAppService>();
    //        JobDataMap dataMap = context.JobDetail.JobDataMap;
    //        GroupConcepts pService = (GroupConcepts)dataMap["pGroupConcepts"];

    //        invoiceCreator.CreateGroupConceptsInvoices(pService);
    //    }
    //}
}
