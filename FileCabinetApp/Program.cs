using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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

        private static FileCabinetMemoryService fileCabinetService = new FileCabinetDefaultService();

        // private static IFileCabinetService fileCabinetStorage = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite));
        private static IFileCabinetService fileCabinetStorage = new FileCabinetMemoryService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export", "allows to export the list of records to different formats", "The 'export' command allows to export the list of records to different formats." },
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

                if (args[0].Remove(0, "--storage=".Length).ToLower(null) == "file")
                {
                    fileCabinetStorage = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite));
                }
            }

            if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "-v":
                        if (args[1].ToLower(null) == "custom")
                        {
                            fileCabinetService = new FileCabinetCustomService();
                            validationRules = "custom";
                        }

                        break;
                    case "-s":
                        if (args[1].ToLower(null) == "file")
                        {
                            fileCabinetStorage = new FileCabinetFilesystemService(new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite));
                        }

                        break;
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
            var name = ReadInput(StringConverter, FirstNameValidator);

            Console.Write("Last name: ");
            var surname = ReadInput(StringConverter, LastNameValidator);

            Console.Write("Date of birth: ");
            var birthday = ReadInput(DateConverter, DateOfBirthValidator);

            Console.Write("Grade: ");
            var grade = ReadInput(ShortConverter, GradeValidator);

            Console.Write("Height: ");
            var height = ReadInput(DecimalConverter, HeightValidator);

            Console.Write("Favourite symbol: ");
            var favouriteSymbol = ReadInput(CharConverter, FavouriteSymbolValidator);

            // var recordId = Program.fileCabinetStorage.CreateRecord(new FileCabinetRecordInfo { FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
            var recordId = Program.fileCabinetService.CreateRecord(new FileCabinetRecordInfo { FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });

            // var result = Program.fileCabinetStorage.CreateRecord(new FileCabinetRecordInfo { Id = recordId, FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            // var listOfRecords = Program.fileCabinetStorage.GetRecords();
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
                        // searchResult = new List<FileCabinetRecord>(Program.fileCabinetStorage.FindByFirstName(args[1].Trim('"')));
                        searchResult = new List<FileCabinetRecord>(Program.fileCabinetService.FindByFirstName(args[1].Trim('"')));
                        break;
                    case "lastname":
                        // searchResult = new List<FileCabinetRecord>(Program.fileCabinetStorage.FindByLastName(args[1].Trim('"')));
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
            // var listOfRecords = new List<FileCabinetRecord>(Program.fileCabinetStorage.GetRecords());
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
                    var name = ReadInput(StringConverter, FirstNameValidator);

                    Console.Write("Last name: ");
                    var surname = ReadInput(StringConverter, LastNameValidator);

                    Console.Write("Date of birth: ");
                    var birthday = ReadInput(DateConverter, DateOfBirthValidator);

                    Console.Write("Grade: ");
                    var grade = ReadInput(ShortConverter, GradeValidator);

                    Console.Write("Height: ");
                    var height = ReadInput(DecimalConverter, HeightValidator);

                    Console.Write("Favourite symbol: ");
                    var favouriteSymbol = ReadInput(CharConverter, FavouriteSymbolValidator);

                    // Program.fileCabinetStorage.EditRecord(new FileCabinetRecordInfo { Id = listOfRecords[index].Id, FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                    Program.fileCabinetService.EditRecord(new FileCabinetRecordInfo { Id = listOfRecords[index].Id, FirstName = name, LastName = surname, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                    Console.WriteLine($"Record #{parameters} is updated.");
                }
            }
        }

        private static void Export(string parameters)
        {
            string[] args = parameters.Split(' ');
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
                                StreamWriter writer = new StreamWriter(new FileStream(args[1], FileMode.Create));
                                var snapshot = Program.fileCabinetService.MakeSnapshot();
                                snapshot.SaveToCsv(writer);
                                Console.WriteLine("All records are exported to file {0}.", args[1]);
                                writer.Close();
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
                                StreamWriter writer = new StreamWriter(new FileStream(args[1], FileMode.Create));
                                var snapshot = Program.fileCabinetService.MakeSnapshot();
                                snapshot.SaveToXml(writer);
                                Console.WriteLine("All records are exported to file {0}.", args[1]);
                                writer.Close();
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

        private static void Stat(string parameters)
        {
            // var recordsCount = Program.fileCabinetStorage.GetStat();
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> StringConverter(string source)
        {
            return new Tuple<bool, string, string>(true, source, source);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string dateOfBirth)
        {
            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "MM'/'dd'/'yyyy", null, DateTimeStyles.None, out birthday);

            return new Tuple<bool, string, DateTime>(dateSuccess, dateOfBirth, birthday);
        }

        private static Tuple<bool, string, short> ShortConverter(string source)
        {
            short grade;
            bool gradeSuccess = short.TryParse(source, out grade);

            return new Tuple<bool, string, short>(gradeSuccess, source, grade);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string source)
        {
            decimal height;
            bool heightSuccess = decimal.TryParse(source, out height);

            return new Tuple<bool, string, decimal>(heightSuccess, source, height);
        }

        private static Tuple<bool, string, char> CharConverter(string source)
        {
            return new Tuple<bool, string, char>(true, source, source[0]);
        }

        private static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (firstName is null || firstName.Trim().Length == 0 || firstName.Length < Program.fileCabinetService.Validator.MinLength || firstName.Length > Program.fileCabinetService.Validator.MaxLength)
            {
                return new Tuple<bool, string>(false, firstName);
            }

            return new Tuple<bool, string>(true, firstName);
        }

        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            if (lastName is null || lastName.Trim().Length == 0 || lastName.Length < Program.fileCabinetService.Validator.MinLength || lastName.Length > Program.fileCabinetService.Validator.MaxLength)
            {
                return new Tuple<bool, string>(false, lastName);
            }

            return new Tuple<bool, string>(true, lastName);
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth < Program.fileCabinetService.Validator.MinimalDate || dateOfBirth > Program.fileCabinetService.Validator.MaximalDate)
            {
                return new Tuple<bool, string>(false, dateOfBirth.ToString("MM'/'dd'/'yyyy", null));
            }

            return new Tuple<bool, string>(true, dateOfBirth.ToString("MM'/'dd'/'yyyy", null));
        }

        private static Tuple<bool, string> GradeValidator(short grade)
        {
            if (grade < Program.fileCabinetService.Validator.MinGrade || grade > Program.fileCabinetService.Validator.MaxGrade)
            {
                return new Tuple<bool, string>(false, grade.ToString());
            }

            return new Tuple<bool, string>(true, grade.ToString());
        }

        private static Tuple<bool, string> HeightValidator(decimal height)
        {
            if (height < Program.fileCabinetService.Validator.MinHeight || height > Program.fileCabinetService.Validator.MaxHeight)
            {
                return new Tuple<bool, string>(false, height.ToString());
            }

            return new Tuple<bool, string>(true, height.ToString());
        }

        private static Tuple<bool, string> FavouriteSymbolValidator(char favouriteSymbol)
        {
            if (favouriteSymbol == ' ')
            {
                return new Tuple<bool, string>(false, favouriteSymbol.ToString());
            }

            return new Tuple<bool, string>(true, favouriteSymbol.ToString());
        }
    }
}
