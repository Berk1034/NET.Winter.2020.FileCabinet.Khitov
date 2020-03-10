﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// The Program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Constantin Hitov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        /// <summary>
        /// The start point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            string validationRules = "default";
            int argsAmount = args.Length;
            IRecordValidator validator = new DefaultValidator();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--validation-rules=", StringComparison.OrdinalIgnoreCase))
                {
                    if (args[i].Remove(0, "--validation-rules=".Length).ToLower(null) == "custom")
                    {
                        validator = new CustomValidator();
                        validationRules = "custom";
                    }
                }

                if (args[i] == "-v")
                {
                    if (args[i + 1].ToLower(null) == "custom")
                    {
                        validator = new CustomValidator();
                        validationRules = "custom";
                    }
                }
            }

            bool foundFlag = false;

            for (int i = 0; i < args.Length && !foundFlag; i += 2)
            {
                if (args[i].Contains("--storage=", StringComparison.OrdinalIgnoreCase))
                {
                    if (args[i].Remove(0, "--storage=".Length).ToLower(null) == "file")
                    {
                        fileCabinetService = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite), validator);
                        foundFlag = true;
                    }
                }
                else if (args[i] == "-s")
                {
                    if (args[i + 1].ToLower(null) == "file")
                    {
                        fileCabinetService = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite), validator);
                        foundFlag = true;
                    }
                }
                else
                {
                    fileCabinetService = new FileCabinetMemoryService(validator);
                }
            }

            if (argsAmount == 0)
            {
                fileCabinetService = new FileCabinetMemoryService(validator);
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            var commandHandler = CreateCommandHandlers();
            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                commandHandler.Handle(
                    new AppCommandRequest
                    {
                        Command = command,
                        Parameters = parameters,
                    });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var recordPrinter = new DefaultRecordPrinter();

            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(Program.fileCabinetService);
            var editHandler = new EditCommandHandler(Program.fileCabinetService);
            var exitHandler = new ExitCommandHandler(Program.fileCabinetService, ChangeIsRunning);
            var exportHandler = new ExportCommandHandler(Program.fileCabinetService);
            var findHandler = new FindCommandHandler(Program.fileCabinetService, recordPrinter);
            var importHandler = new ImportCommandHandler(Program.fileCabinetService);
            var listHandler = new ListCommandHandler(Program.fileCabinetService, recordPrinter);
            var purgeHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var removeHandler = new RemoveCommandHandler(Program.fileCabinetService);
            var statHandler = new StatCommandHandler(Program.fileCabinetService);
            var missedHandler = new MissedCommandHandler();

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(editHandler);
            editHandler.SetNext(removeHandler);
            removeHandler.SetNext(findHandler);
            findHandler.SetNext(statHandler);
            statHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(listHandler);
            listHandler.SetNext(importHandler);
            importHandler.SetNext(exportHandler);
            exportHandler.SetNext(exitHandler);
            exitHandler.SetNext(missedHandler);

            return helpHandler;
        }

        private static void ChangeIsRunning(bool value)
        {
            isRunning = value;
        }
    }
}
