﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The RemoveCommandHandler class.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private bool useStopWatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public RemoveCommandHandler(IFileCabinetService service, bool useStopWatch)
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
            if (appCommandRequest.Command == "remove")
            {
                var listOfRecords = new List<FileCabinetRecord>(this.service.GetRecords());
                int removeId;
                bool parseSuccess = int.TryParse(appCommandRequest.Parameters, out removeId);
                if (!parseSuccess)
                {
                    Console.WriteLine("You typed invalid symbols!");
                }
                else
                {
                    int index = listOfRecords.FindIndex((record) => record.Id == removeId);
                    if (index == -1)
                    {
                        Console.WriteLine($"Record #{appCommandRequest.Parameters} doesn't exist.");
                    }
                    else
                    {
                        if (this.useStopWatch)
                        {
                            ServiceMeter serviceMeter = new ServiceMeter(this.service);
                            serviceMeter.Remove(removeId);
                        }
                        else
                        {
                            this.service.Remove(removeId);
                        }

                        Console.WriteLine($"Record #{removeId} is removed.");
                    }
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
