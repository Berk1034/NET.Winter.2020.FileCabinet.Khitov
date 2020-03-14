using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The RemoveCommandHandler class.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public RemoveCommandHandler(IFileCabinetService service, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
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
                        if (this.serviceLogger != null)
                        {
                            this.serviceLogger.Remove(removeId);
                        }
                        else if (this.serviceMeter != null)
                        {
                            this.serviceMeter.Remove(removeId);
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
