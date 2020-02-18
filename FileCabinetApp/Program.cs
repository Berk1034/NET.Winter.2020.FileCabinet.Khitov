using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// The Program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Constantin Hitov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static IFileCabinetService fileCabinetService = new FileCabinetDefaultService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "creates the record", "The 'create' command creates the record." },
            new string[] { "find", "finds the record by specified property", "The 'find' command finds the record by specified property." },
            new string[] { "edit", "allows to update the choosen record", "The 'edit' command allows to update the choosen record." },
            new string[] { "list", "provides the list of records", "The 'list' command provides the list of records." },
            new string[] { "stat", "provides the statistics of records", "The 'stat' command provides the statistics of records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <summary>
        /// The start point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            string validationRules = "default";

            if (args.Length == 1)
            {
                if (args[0].Remove(0, "--validation-rules=".Length).ToLower(null) == "custom")
                {
                    fileCabinetService = new FileCabinetCustomService();
                    validationRules = "custom";
                }
                else
                {
                    fileCabinetService = new FileCabinetDefaultService();
                }
            }

            if (args.Length == 2 && args[0] == "-v")
            {
                if (args[1].ToLower(null) == "custom")
                {
                    fileCabinetService = new FileCabinetCustomService();
                    validationRules = "custom";
                }
                else
                {
                    fileCabinetService = new FileCabinetDefaultService();
                }
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string name = Console.ReadLine();
            while (name is null || name.Trim().Length == 0 || name.Length < 2 || name.Length > 60)
            {
                Console.WriteLine($"Invalid input! First name can't be empty and it should contain from {2} to {60} non-space symbols. Try again.");
                Console.Write("First name: ");
                name = Console.ReadLine();
            }

            Console.Write("Last name: ");
            string surname = Console.ReadLine();
            while (surname is null || surname.Trim().Length == 0 || surname.Length < 2 || surname.Length > 60)
            {
                Console.WriteLine($"Invalid input! Last name can't be empty and it should contain from {2} to {60} non-space symbols. Try again.");
                Console.Write("Last name: ");
                surname = Console.ReadLine();
            }

            Console.Write("Date of birth: ");

            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, DateTimeStyles.None, out birthday);
            while ((!dateSuccess) || (birthday < new DateTime(1950, 1, 1) || birthday > DateTime.Now))
            {
                Console.WriteLine($"Invalid input! Date of birth should be in format MM/dd/yyyy and minimal date is {new DateTime(1950, 1, 1).ToString("MM'/'dd'/'yyyy", null)}. Try again.");
                Console.Write("Date of birth: ");
                dateSuccess = DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, DateTimeStyles.None, out birthday);
            }

            Console.Write("Grade: ");
            short grade;
            bool gradeSuccess = short.TryParse(Console.ReadLine(), out grade);
            while ((!gradeSuccess) || (grade < -10 || grade > 10))
            {
                Console.WriteLine($"Invalid input! Grade should be integer number in range from {-10} to {10}. Try again.");
                Console.Write("Grade: ");
                gradeSuccess = short.TryParse(Console.ReadLine(), out grade);
            }

            Console.Write("Height: ");
            decimal height;
            bool heightSuccess = decimal.TryParse(Console.ReadLine(), out height);
            while ((!heightSuccess) || (height < 0.3m || height > 3m))
            {
                Console.WriteLine($"Invalid input! Height should be number in range from {0.3m} to {3m}. Try again.");
                Console.Write("Height: ");
                heightSuccess = decimal.TryParse(Console.ReadLine(), out height);
            }

            Console.Write("Favourite symbol: ");
            char favouriteSymbol = Console.ReadLine()[0];
            while (favouriteSymbol == ' ')
            {
                Console.WriteLine("Invalid input! Space symbol is not valid. Try again.");
                Console.Write("Favourite symbol: ");
                favouriteSymbol = Console.ReadLine()[0];
            }

            var recordId = Program.fileCabinetService.CreateRecord(new FileCabinetRecordInfo { FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            var listOfRecords = Program.fileCabinetService.GetRecords();
            foreach (var record in listOfRecords)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {record.Grade}, {record.Height}, {record.FavouriteSymbol}");
            }
        }

        private static void Find(string parameters)
        {
            string[] args = parameters.Split(' ');
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
                    Console.WriteLine($"#{searchResult[i].Id}, {searchResult[i].FirstName}, {searchResult[i].LastName}, {searchResult[i].DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {searchResult[i].Grade}, {searchResult[i].Height}, {searchResult[i].FavouriteSymbol}");
                }
            }
            else
            {
                Console.WriteLine("This command admits two arguments! Try again.");
            }
        }

        private static void Edit(string parameters)
        {
            var listOfRecords = new List<FileCabinetRecord>(Program.fileCabinetService.GetRecords());
            int editId;
            bool parseSuccess = int.TryParse(parameters, out editId);
            if (!parseSuccess)
            {
                Console.WriteLine("You typed invalid symbols!");
            }
            else
            {
                int index = listOfRecords.FindIndex((record) => record.Id == editId);
                if (index == -1)
                {
                    Console.WriteLine($"#{parameters} record is not found.");
                }
                else
                {
                    Console.Write("First name: ");
                    string name = Console.ReadLine();
                    while (name is null || name.Trim().Length == 0 || name.Length < 2 || name.Length > 60)
                    {
                        Console.WriteLine($"Invalid input! First name can't be empty and it should contain from {2} to {60} non-space symbols. Try again.");
                        Console.Write("First name: ");
                        name = Console.ReadLine();
                    }

                    Console.Write("Last name: ");
                    string surname = Console.ReadLine();
                    while (surname is null || surname.Trim().Length == 0 || surname.Length < 2 || surname.Length > 60)
                    {
                        Console.WriteLine($"Invalid input! Last name can't be empty and it should contain from {2} to {60} non-space symbols. Try again.");
                        Console.Write("Last name: ");
                        surname = Console.ReadLine();
                    }

                    Console.Write("Date of birth: ");

                    DateTime birthday;
                    bool dateSuccess = DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, DateTimeStyles.None, out birthday);
                    while ((!dateSuccess) || (birthday < new DateTime(1950, 1, 1) || birthday > DateTime.Now))
                    {
                        Console.WriteLine($"Invalid input! Date of birth should be in format MM/dd/yyyy and minimal date is {new DateTime(1950, 1, 1).ToString("MM'/'dd'/'yyyy", null)}. Try again.");
                        Console.Write("Date of birth: ");
                        dateSuccess = DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, DateTimeStyles.None, out birthday);
                    }

                    Console.Write("Grade: ");
                    short grade;
                    bool gradeSuccess = short.TryParse(Console.ReadLine(), out grade);
                    while ((!gradeSuccess) || (grade < -10 || grade > 10))
                    {
                        Console.WriteLine($"Invalid input! Grade should be integer number in range from {-10} to {10}. Try again.");
                        Console.Write("Grade: ");
                        gradeSuccess = short.TryParse(Console.ReadLine(), out grade);
                    }

                    Console.Write("Height: ");
                    decimal height;
                    bool heightSuccess = decimal.TryParse(Console.ReadLine(), out height);
                    while ((!heightSuccess) || (height < 0.3m || height > 3m))
                    {
                        Console.WriteLine($"Invalid input! Height should be number in range from {0.3m} to {3m}. Try again.");
                        Console.Write("Height: ");
                        heightSuccess = decimal.TryParse(Console.ReadLine(), out height);
                    }

                    Console.Write("Favourite symbol: ");
                    char favouriteSymbol = Console.ReadLine()[0];
                    while (favouriteSymbol == ' ')
                    {
                        Console.WriteLine("Invalid input! Space symbol is not valid. Try again.");
                        Console.Write("Favourite symbol: ");
                        favouriteSymbol = Console.ReadLine()[0];
                    }

                    Program.fileCabinetService.EditRecord(new FileCabinetRecordInfo { Id = listOfRecords[index].Id, FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                    Console.WriteLine($"Record #{parameters} is updated.");
                }
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}
