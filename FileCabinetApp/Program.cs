using System;
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
        private static ValidationRules validationRules;
        private static IPrinter printer;

        /// <summary>
        /// Gets the validationRules.
        /// </summary>
        /// <value>
        /// The validationRules.
        /// </value>
        public static ValidationRules ValidationRules
        {
            get => validationRules;
        }

        /// <summary>
        /// Gets the fileCabinetService.
        /// </summary>
        /// <value>
        /// The fileCabinetService.
        /// </value>
        public static IFileCabinetService FileCabinetService
        {
            get => fileCabinetService;
        }

        /// <summary>
        /// The start point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args), "Arguments is null.");
            }

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
                    fileCabinetService = new ServiceMeter(fileCabinetService);
                }

                if (args[i] == "use-logger")
                {
                    fileCabinetService = new ServiceLogger(new FileStream("Log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite), new ServiceMeter(fileCabinetService));
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

            printer = new TablePrinter();

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
            var createHandler = new CreateCommandHandler(Program.fileCabinetService);
            var insertHandler = new InsertCommandHandler(Program.fileCabinetService);
            var updateHandler = new UpdateCommandHandler(Program.fileCabinetService);
            var exitHandler = new ExitCommandHandler(Program.fileCabinetService, Program.ChangeIsRunning);
            var exportHandler = new ExportCommandHandler(Program.fileCabinetService);
            var importHandler = new ImportCommandHandler(Program.fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(Program.fileCabinetService);
            var statHandler = new StatCommandHandler(Program.fileCabinetService);
            var selectHandler = new SelectCommandHanlder(Program.fileCabinetService, Program.printer);
            var missedHandler = new MissedCommandHandler();

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(updateHandler);
            updateHandler.SetNext(insertHandler);
            insertHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(selectHandler);
            selectHandler.SetNext(statHandler);
            statHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(importHandler);
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
