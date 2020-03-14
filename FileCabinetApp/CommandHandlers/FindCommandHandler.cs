using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The FindCommandHandler class.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="printer">The IRecordPrinter printer.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
            : base(service)
        {
            this.printer = printer;
            this.serviceMeter = serviceMeter;
            this.serviceLogger = serviceLogger;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "find")
            {
                string[] args = appCommandRequest.Parameters.Split(' ');
                if (args.Length == 2)
                {
                    List<FileCabinetRecord> searchResult = new List<FileCabinetRecord>();
                    switch (args[0].ToLower(null))
                    {
                        case "firstname":
                            if (this.serviceLogger != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceLogger.FindByFirstName(args[1].Trim('"')));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceMeter.FindByFirstName(args[1].Trim('"')));
                            }
                            else
                            {
                                searchResult = new List<FileCabinetRecord>(this.service.FindByFirstName(args[1].Trim('"')));
                            }

                            break;
                        case "lastname":
                            if (this.serviceLogger != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceLogger.FindByLastName(args[1].Trim('"')));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceMeter.FindByLastName(args[1].Trim('"')));
                            }
                            else
                            {
                                searchResult = new List<FileCabinetRecord>(this.service.FindByLastName(args[1].Trim('"')));
                            }

                            break;
                        case "dateofbirth":
                            if (this.serviceLogger != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceLogger.FindByDateOfBirth(args[1].Trim('"')));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = new List<FileCabinetRecord>(this.serviceMeter.FindByDateOfBirth(args[1].Trim('"')));
                            }
                            else
                            {
                                searchResult = new List<FileCabinetRecord>(this.service.FindByDateOfBirth(args[1].Trim('"')));
                            }

                            break;
                        default:
                            Console.WriteLine("Nothing found.");
                            break;
                    }

                    this.printer(searchResult);
                }
                else
                {
                    Console.WriteLine("This command admits two arguments! Try again.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
