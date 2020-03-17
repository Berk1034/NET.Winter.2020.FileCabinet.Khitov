﻿using System;
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
                    IRecordIterator searchResult = null;
                    switch (args[0].ToLower(null))
                    {
                        case "firstname":
                            if (this.serviceLogger != null)
                            {
                                searchResult = this.serviceLogger.FindByFirstName(args[1].Trim('"'));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = this.serviceMeter.FindByFirstName(args[1].Trim('"'));
                            }
                            else
                            {
                                searchResult = this.service.FindByFirstName(args[1].Trim('"'));
                            }

                            break;
                        case "lastname":
                            if (this.serviceLogger != null)
                            {
                                searchResult = this.serviceLogger.FindByLastName(args[1].Trim('"'));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = this.serviceMeter.FindByLastName(args[1].Trim('"'));
                            }
                            else
                            {
                                searchResult = this.service.FindByLastName(args[1].Trim('"'));
                            }

                            break;
                        case "dateofbirth":
                            if (this.serviceLogger != null)
                            {
                                searchResult = this.serviceLogger.FindByDateOfBirth(args[1].Trim('"'));
                            }
                            else if (this.serviceMeter != null)
                            {
                                searchResult = this.serviceMeter.FindByDateOfBirth(args[1].Trim('"'));
                            }
                            else
                            {
                                searchResult = this.service.FindByDateOfBirth(args[1].Trim('"'));
                            }

                            break;
                        default:
                            break;
                    }

                    //this.printer(searchResult2);
                    if (searchResult != null)
                    {
                        while (searchResult.HasMore())
                        {
                            var record = searchResult.GetNext();
                            Console.WriteLine($"#{record.Id}, {record.Name.FirstName}, {record.Name.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {record.Grade}, {record.Height}, {record.FavouriteSymbol}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nothing found.");
                    }
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
