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

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="printer">The IRecordPrinter printer.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
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
                    IEnumerable<FileCabinetRecord> searchResult = null;
                    switch (args[0].ToLower(null))
                    {
                        case "firstname":
                            searchResult = this.service.FindByFirstName(args[1].Trim('"'));

                            break;
                        case "lastname":
                            searchResult = this.service.FindByLastName(args[1].Trim('"'));

                            break;
                        case "dateofbirth":
                            searchResult = this.service.FindByDateOfBirth(args[1].Trim('"'));

                            break;
                        default:
                            break;
                    }

                    if (searchResult != null)
                    {
                        this.printer(searchResult);
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
