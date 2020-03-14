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
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public StatCommandHandler(IFileCabinetService service, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
            : base(service)
        {
            this.serviceMeter = serviceMeter;
            this.serviceLogger = serviceLogger;
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

                if (this.serviceLogger != null)
                {
                    (totalRecordsCount, deletedRecordsCount) = this.serviceLogger.GetStat();
                }
                else if (this.serviceMeter != null)
                {
                    (totalRecordsCount, deletedRecordsCount) = this.serviceMeter.GetStat();
                }
                else
                {
                    (totalRecordsCount, deletedRecordsCount) = this.service.GetStat();
                }

                Console.WriteLine($"Totally {totalRecordsCount} record(s). Need to delete {deletedRecordsCount} record(s).");
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
