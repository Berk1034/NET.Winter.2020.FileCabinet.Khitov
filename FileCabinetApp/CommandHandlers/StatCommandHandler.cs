using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The StatCommandHandler class.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private bool useStopWatch;
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public StatCommandHandler(IFileCabinetService service, bool useStopWatch)
            : base(service)
        {
            this.useStopWatch = useStopWatch;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "stat")
            {
                int totalRecordsCount;
                int deletedRecordsCount;

                if (this.useStopWatch)
                {
                    ServiceMeter serviceMeter = new ServiceMeter(this.service);
                    (totalRecordsCount, deletedRecordsCount) = serviceMeter.GetStat();
                }
                else
                {
                    (totalRecordsCount, deletedRecordsCount) = this.service.GetStat();
                }

                /*
                var totalRecordsCount = this.service.GetStat().total;
                var deletedRecordsCount = this.service.GetStat().deleted;
                */
                Console.WriteLine($"Totally {totalRecordsCount} record(s). Need to delete {deletedRecordsCount} record(s).");
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
