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
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
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
                                    StreamWriter writer = null;
                                    try
                                    {
                                        writer = new StreamWriter(new FileStream(args[1], FileMode.Create));

                                        FileCabinetServiceSnapshot snapshot;
                                        snapshot = this.service.MakeSnapshot();

                                        snapshot.SaveToCsv(writer);
                                        Console.WriteLine("All records are exported to file {0}.", args[1]);
                                        writer.Close();
                                    }
                                    catch (ArgumentException)
                                    {
                                        Console.WriteLine("Export failed: empty filepath is not allowed.");
                                    }
                                    catch (NotImplementedException)
                                    {
                                        Console.WriteLine("Can't export data from FilesystemService.");
                                    }
                                    finally
                                    {
                                        if (writer != null)
                                        {
                                            writer.Close();
                                        }
                                    }
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
                                    StreamWriter writer = null;
                                    try
                                    {
                                        writer = new StreamWriter(new FileStream(args[1], FileMode.Create));

                                        FileCabinetServiceSnapshot snapshot;
                                        snapshot = this.service.MakeSnapshot();

                                        snapshot.SaveToXml(writer);
                                        Console.WriteLine("All records are exported to file {0}.", args[1]);
                                        writer.Close();
                                    }
                                    catch (ArgumentException)
                                    {
                                        Console.WriteLine("Export failed: empty filepath is not allowed.");
                                    }
                                    catch (NotImplementedException)
                                    {
                                        Console.WriteLine("Can't export data from FilesystemService.");
                                    }
                                    finally
                                    {
                                        if (writer != null)
                                        {
                                            writer.Close();
                                        }
                                    }
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
