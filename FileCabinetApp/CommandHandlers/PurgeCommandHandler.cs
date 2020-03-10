using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The PurgeCommandHandler class.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "purge")
            {
                if (Program.fileCabinetService is FileCabinetFilesystemService)
                {
                    var totalAmountOfRecords = Program.fileCabinetService.GetStat().total;
                    Program.fileCabinetService.Purge();
                    var purgedRecords = totalAmountOfRecords - Program.fileCabinetService.GetStat().total;
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
