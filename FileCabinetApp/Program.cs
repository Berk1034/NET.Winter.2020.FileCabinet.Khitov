﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

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
        private static bool useStopWatch = false;
        private static ValidationRules validationRules;

        /// <summary>
        /// The start point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            string validation = "default";
            int argsAmount = args.Length;
            IRecordValidator validator = new ValidatorBuilder().CreateDefault();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--validation-rules=", StringComparison.OrdinalIgnoreCase))
                {
                    if (args[i].Remove(0, "--validation-rules=".Length).ToLower(null) == "custom")
                    {
                        validator = new ValidatorBuilder().CreateCustom();
                        validation = "custom";
                    }
                }

                if (args[i] == "-v")
                {
                    if (args[i + 1].ToLower(null) == "custom")
                    {
                        validator = new ValidatorBuilder().CreateCustom();
                        validation = "custom";
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

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "use-stopwatch")
                {
                    useStopWatch = true;
                }
            }

            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var builder = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("validation-rules.json");
            var config = builder.Build();
            validationRules = config.GetSection(validation).Get<ValidationRules>();

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validation} validation rules.");
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
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(Program.fileCabinetService, validationRules, useStopWatch);
            var editHandler = new EditCommandHandler(Program.fileCabinetService, validationRules, useStopWatch);
            var exitHandler = new ExitCommandHandler(Program.fileCabinetService, Program.ChangeIsRunning);
            var exportHandler = new ExportCommandHandler(Program.fileCabinetService, useStopWatch);
            var findHandler = new FindCommandHandler(Program.fileCabinetService, Program.DefaultRecordPrint, useStopWatch);
            var importHandler = new ImportCommandHandler(Program.fileCabinetService, useStopWatch);
            var listHandler = new ListCommandHandler(Program.fileCabinetService, Program.DefaultRecordPrint, useStopWatch);
            var purgeHandler = new PurgeCommandHandler(Program.fileCabinetService, useStopWatch);
            var removeHandler = new RemoveCommandHandler(Program.fileCabinetService, useStopWatch);
            var statHandler = new StatCommandHandler(Program.fileCabinetService, useStopWatch);
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

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.Name.FirstName}, {record.Name.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {record.Grade}, {record.Height}, {record.FavouriteSymbol}");
            }
        }
    }
}
