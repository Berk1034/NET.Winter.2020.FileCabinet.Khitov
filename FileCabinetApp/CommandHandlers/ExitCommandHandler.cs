﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ExitCommandHandler class.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public ExitCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "exit")
            {
                if (this.service is FileCabinetFilesystemService)
                {
                    (this.service as FileCabinetFilesystemService).Dispose();
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
