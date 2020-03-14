using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ImportCommandHandler class.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public ImportCommandHandler(IFileCabinetService service, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
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
            if (appCommandRequest.Command == "import")
            {
                string[] args = appCommandRequest.Parameters.Split(' ');
                if (args.Length == 2)
                {
                    switch (args[0])
                    {
                        case "csv":
                            try
                            {
                                StreamReader reader = new StreamReader(new FileStream(args[1], FileMode.Open));
                                FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                                snapshot.LoadFromCsv(reader);

                                int importedRecordsCount;
                                if (this.serviceLogger != null)
                                {
                                    importedRecordsCount = this.serviceLogger.Restore(snapshot);
                                }
                                else if (this.serviceMeter != null)
                                {
                                    importedRecordsCount = this.serviceMeter.Restore(snapshot);
                                }
                                else
                                {
                                    importedRecordsCount = this.service.Restore(snapshot);
                                }

                                Console.WriteLine("{0} records were imported from {1}.", importedRecordsCount, args[1]);
                                reader.Close();
                            }
                            catch (DirectoryNotFoundException)
                            {
                                Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                            }
                            catch (FileNotFoundException)
                            {
                                Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                            }

                            break;
                        case "xml":
                            try
                            {
                                StreamReader reader = new StreamReader(new FileStream(args[1], FileMode.Open));
                                FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                                snapshot.LoadFromXml(reader);

                                int importedRecordsCount;
                                if (this.serviceLogger != null)
                                {
                                    importedRecordsCount = this.serviceLogger.Restore(snapshot);
                                }
                                else if (this.serviceMeter != null)
                                {
                                    importedRecordsCount = this.serviceMeter.Restore(snapshot);
                                }
                                else
                                {
                                    importedRecordsCount = this.service.Restore(snapshot);
                                }

                                Console.WriteLine("{0} records were imported from {1}.", importedRecordsCount, args[1]);
                                reader.Close();
                            }
                            catch (DirectoryNotFoundException)
                            {
                                Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                            }
                            catch (FileNotFoundException)
                            {
                                Console.WriteLine("Import error: file {0} is not exist.", args[1]);
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
