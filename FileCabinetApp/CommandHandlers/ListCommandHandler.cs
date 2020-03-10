using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ListCommandHandler class.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="printer">The IRecordPrinter printer.</param>
        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "list")
            {
                var listOfRecords = this.service.GetRecords();
                this.printer.Print(listOfRecords);
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
