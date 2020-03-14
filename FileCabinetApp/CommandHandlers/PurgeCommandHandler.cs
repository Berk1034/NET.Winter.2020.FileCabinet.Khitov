using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The PurgeCommandHandler class.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public PurgeCommandHandler(IFileCabinetService service, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
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
            if (appCommandRequest.Command == "purge")
            {
                if (this.service is FileCabinetFilesystemService)
                {
                    var totalAmountOfRecords = this.service.GetStat().total;

                    if (this.serviceLogger != null)
                    {
                        this.serviceLogger.Purge();
                    }
                    else if (this.serviceMeter != null)
                    {
                        this.serviceMeter.Purge();
                    }
                    else
                    {
                        this.service.Purge();
                    }

                    var purgedRecords = totalAmountOfRecords - this.service.GetStat().total;
                    Console.WriteLine($"Data file processing is completed: {purgedRecords} of {totalAmountOfRecords} were purged.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
