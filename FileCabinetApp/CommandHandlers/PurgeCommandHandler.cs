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
        private bool useStopWatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public PurgeCommandHandler(IFileCabinetService service, bool useStopWatch)
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
            if (appCommandRequest.Command == "purge")
            {
                if (this.service is FileCabinetFilesystemService)
                {
                    var totalAmountOfRecords = this.service.GetStat().total;

                    if (this.useStopWatch)
                    {
                        ServiceMeter serviceMeter = new ServiceMeter(this.service);
                        serviceMeter.Purge();
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
