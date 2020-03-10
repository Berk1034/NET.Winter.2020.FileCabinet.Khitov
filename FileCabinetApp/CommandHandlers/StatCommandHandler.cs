using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The StatCommandHandler class.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "stat")
            {
                var totalRecordsCount = Program.fileCabinetService.GetStat().total;
                var deletedRecordsCount = Program.fileCabinetService.GetStat().deleted;
                Console.WriteLine($"Totally {totalRecordsCount} record(s). Need to delete {deletedRecordsCount} record(s).");
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
