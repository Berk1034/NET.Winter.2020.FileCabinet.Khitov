using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The FindCommandHandler class.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
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
                            searchResult = new List<FileCabinetRecord>(Program.fileCabinetService.FindByFirstName(args[1].Trim('"')));
                            break;
                        case "lastname":
                            searchResult = new List<FileCabinetRecord>(Program.fileCabinetService.FindByLastName(args[1].Trim('"')));
                            break;
                        case "dateofbirth":
                            searchResult = new List<FileCabinetRecord>(Program.fileCabinetService.FindByDateOfBirth(args[1].Trim('"')));
                            break;
                        default:
                            Console.WriteLine("Nothing found.");
                            break;
                    }

                    for (int i = 0; i < searchResult.Count; i++)
                    {
                        Console.WriteLine($"#{searchResult[i].Id}, {searchResult[i].Name.FirstName}, {searchResult[i].Name.LastName}, {searchResult[i].DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {searchResult[i].Grade}, {searchResult[i].Height}, {searchResult[i].FavouriteSymbol}");
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
