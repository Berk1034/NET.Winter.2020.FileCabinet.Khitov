﻿using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The StatCommandHandler class.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest), "AppCommandRequest is null.");
            }

            if (appCommandRequest.Command == "stat")
            {
                int totalRecordsCount;
                int deletedRecordsCount;

                (totalRecordsCount, deletedRecordsCount) = this.Service.GetStat();

                Console.WriteLine($"Totally {totalRecordsCount} record(s). Need to delete {deletedRecordsCount} record(s).");
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
