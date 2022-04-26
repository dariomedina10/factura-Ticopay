using Abp.Domain.Entities.Auditing;
using Quartz;
using System;

namespace TicoPay.Services
{
    public abstract class Event : AuditedEntity<Guid>
    {
        public string CronExpression { get; protected set; }
        public DateTime DateEvent { get; protected set; }

        /// <summary>
        /// Returns the next time at which the Quartz.ITrigger will fire, after the given time. If the trigger will not fire after the given time, null will be returned.
        /// </summary>
        public DateTimeOffset GetNextEvent()
        {
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(CronExpression)
                .Build();

            return trigger.GetFireTimeAfter(DateEvent).GetValueOrDefault();
        }
    }
}
