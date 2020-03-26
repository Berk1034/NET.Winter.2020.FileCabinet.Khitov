using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The MissedCommandHandler class.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        private static string[] availableCommands = new string[]
        {
            "create",
            "insert",
            "update",
            "delete",
            "select",
            "stat",
            "purge",
            "import",
            "export",
            "help",
            "exit",
        };

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            Console.WriteLine($"There is no '{appCommandRequest.Command}' command. See 'help'. ");
            Console.WriteLine();

            IEnumerable<string> mostSimilarCommands = new List<string>();

            string command = appCommandRequest.Command;
            while (!mostSimilarCommands.Any() && command.Length > 0)
            {
                string commandSubstr = command;
                mostSimilarCommands = availableCommands.Where(command => command.StartsWith(commandSubstr, StringComparison.InvariantCulture));
                command = command.Substring(0, command.Length - 1);
            }

            int commandsCount = mostSimilarCommands.Count();

            if (mostSimilarCommands.Any())
            {
                if (commandsCount == 1)
                {
                    Console.WriteLine("The most similar command is");
                }
                else
                {
                    Console.WriteLine("The most similar commands are");
                }

                foreach (var similarCommand in mostSimilarCommands)
                {
                    Console.WriteLine(similarCommand.PadLeft(similarCommand.Length + 8));
                }
            }
        }
    }
}
