using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ExitCommandHandler class.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "exit")
            {
                if (Program.fileCabinetService is FileCabinetFilesystemService)
                {
                    (Program.fileCabinetService as FileCabinetFilesystemService).Dispose();
                }

                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
