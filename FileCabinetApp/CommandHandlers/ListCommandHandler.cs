using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ListCommandHandler class.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;
        private bool useStopWatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="printer">The IRecordPrinter printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer, bool useStopWatch)
            : base(service)
        {
            this.printer = printer;
            this.useStopWatch = useStopWatch;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "list")
            {
                ReadOnlyCollection<FileCabinetRecord> listOfRecords;

                if (this.useStopWatch)
                {
                    ServiceMeter serviceMeter = new ServiceMeter(this.service);
                    listOfRecords = serviceMeter.GetRecords();
                }
                else
                {
                    listOfRecords = this.service.GetRecords();
                }

                this.printer(listOfRecords);
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
