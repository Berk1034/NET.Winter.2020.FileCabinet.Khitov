using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ExportCommandHandler class.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public ExportCommandHandler(IFileCabinetService service, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
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
            if (appCommandRequest.Command == "export")
            {
                string[] args = appCommandRequest.Parameters.Split(' ');
                if (args.Length == 2)
                {
                    switch (args[0])
                    {
                        case "csv":
                            try
                            {
                                bool rewriteFlag = true;
                                if (File.Exists(args[1]))
                                {
                                    Console.Write(@"File exists - rewrite {0}? [Y\n] ", args[1]);
                                    var result = Console.ReadLine();
                                    if (result.Length == 0 || result[0] == 'n')
                                    {
                                        rewriteFlag = false;
                                    }
                                }

                                if (rewriteFlag)
                                {
                                    StreamWriter writer = new StreamWriter(new FileStream(args[1], FileMode.Create));

                                    FileCabinetServiceSnapshot snapshot;
                                    if (this.serviceLogger != null)
                                    {
                                        snapshot = this.serviceLogger.MakeSnapshot();
                                    }
                                    else if (this.serviceMeter != null)
                                    {
                                        snapshot = this.serviceMeter.MakeSnapshot();
                                    }
                                    else
                                    {
                                        snapshot = this.service.MakeSnapshot();
                                    }

                                    snapshot.SaveToCsv(writer);
                                    Console.WriteLine("All records are exported to file {0}.", args[1]);
                                    writer.Close();
                                }
                            }
                            catch (DirectoryNotFoundException)
                            {
                                Console.WriteLine("Export failed: can't open file {0}.", args[1]);
                            }

                            break;
                        case "xml":
                            try
                            {
                                bool rewriteFlag = true;
                                if (File.Exists(args[1]))
                                {
                                    Console.Write(@"File exists - rewrite {0}? [Y\n] ", args[1]);
                                    var result = Console.ReadLine();
                                    if (result.Length == 0 || result[0] == 'n')
                                    {
                                        rewriteFlag = false;
                                    }
                                }

                                if (rewriteFlag)
                                {
                                    StreamWriter writer = new StreamWriter(new FileStream(args[1], FileMode.Create));

                                    FileCabinetServiceSnapshot snapshot;
                                    if (this.serviceLogger != null)
                                    {
                                        snapshot = this.serviceLogger.MakeSnapshot();
                                    }
                                    else if (this.serviceMeter != null)
                                    {
                                        snapshot = this.serviceMeter.MakeSnapshot();
                                    }
                                    else
                                    {
                                        snapshot = this.service.MakeSnapshot();
                                    }

                                    snapshot.SaveToXml(writer);
                                    Console.WriteLine("All records are exported to file {0}.", args[1]);
                                    writer.Close();
                                }
                            }
                            catch (DirectoryNotFoundException)
                            {
                                Console.WriteLine("Export failed: can't open file {0}.", args[1]);
                            }

                            break;
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
