using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace ContactsNotificationPublisher
{
    public class QuartzStartup
    {
        private readonly IServiceProvider serviceProvider;
        private IScheduler scheduler;

        public QuartzStartup(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Start()
        {
            var schedulerFactory = new StdSchedulerFactory();

            scheduler = await schedulerFactory.GetScheduler();
            scheduler.JobFactory = new JobFactory(serviceProvider);

            await scheduler.Start();
            
            var notificationPublishJob = JobBuilder.Create<NotificationPublisher>().Build();
            var trigger =
                TriggerBuilder
                    .Create()
                    .StartNow()
                    .WithSimpleSchedule(scheduleBuilder =>
                        scheduleBuilder
                        .WithInterval(TimeSpan.FromSeconds(5))
                        .RepeatForever())
                    .Build();

            scheduler.ScheduleJob(notificationPublishJob, trigger).GetAwaiter().GetResult();
        }
    }
}