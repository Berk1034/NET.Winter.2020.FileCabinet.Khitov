using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The RemoveCommandHandler class.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "remove")
            {
                var listOfRecords = new List<FileCabinetRecord>(Program.fileCabinetService.GetRecords());
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
                        Program.fileCabinetService.Remove(removeId);
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
