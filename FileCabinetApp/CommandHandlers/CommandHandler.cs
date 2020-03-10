using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The CommandHandler class.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("purge", Purge),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "creates the record", "The 'create' command creates the record." },
            new string[] { "find", "finds the record by specified property", "The 'find' command finds the record by specified property." },
            new string[] { "edit", "allows to update the choosen record", "The 'edit' command allows to update the choosen record." },
            new string[] { "remove", "allows to remove the choosen record", "The 'remove' command allows to remove the choosen record." },
            new string[] { "list", "provides the list of records", "The 'list' command provides the list of records." },
            new string[] { "stat", "provides the statistics of records", "The 'stat' command provides the statistics of records." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
            new string[] { "import", "allows to import the list of records from different formats", "The 'import' command allows to import the list of records from different formats." },
            new string[] { "export", "allows to export the list of records to different formats", "The 'export' command allows to export the list of records to different formats." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(appCommandRequest.Command, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                commands[index].Item2(appCommandRequest.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(appCommandRequest.Command);
            }
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
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

            var recordId = Program.fileCabinetService.CreateRecord(new FileCabinetRecord { Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            var listOfRecords = Program.fileCabinetService.GetRecords();

            foreach (var record in listOfRecords)
            {
                Console.WriteLine($"#{record.Id}, {record.Name.FirstName}, {record.Name.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {record.Grade}, {record.Height}, {record.FavouriteSymbol}");
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
                    Console.WriteLine($"#{searchResult[i].Id}, {searchResult[i].Name.FirstName}, {searchResult[i].Name.LastName}, {searchResult[i].DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {searchResult[i].Grade}, {searchResult[i].Height}, {searchResult[i].FavouriteSymbol}");
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

                    Program.fileCabinetService.EditRecord(new FileCabinetRecord { Id = listOfRecords[index].Id, Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                    Console.WriteLine($"Record #{parameters} is updated.");
                }
            }
        }

        private static void Remove(string parameters)
        {
            var listOfRecords = new List<FileCabinetRecord>(Program.fileCabinetService.GetRecords());
            int removeId;
            bool parseSuccess = int.TryParse(parameters, out removeId);
            if (!parseSuccess)
            {
                Console.WriteLine("You typed invalid symbols!");
            }
            else
            {
                int index = listOfRecords.FindIndex((record) => record.Id == removeId);
                if (index == -1)
                {
                    Console.WriteLine($"Record #{parameters} doesn't exist.");
                }
                else
                {
                    Program.fileCabinetService.Remove(removeId);
                    Console.WriteLine($"Record #{removeId} is removed.");
                }
            }
        }

        private static void Import(string parameters)
        {
            string[] args = parameters.Split(' ');
            if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "csv":
                        try
                        {
                            StreamReader reader = new StreamReader(new FileStream(args[1], FileMode.Open));
                            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                            snapshot.LoadFromCsv(reader);
                            int importedRecordsCount = Program.fileCabinetService.Restore(snapshot);
                            Console.WriteLine("{0} records were imported from {1}.", importedRecordsCount, args[1]);
                            reader.Close();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                        }

                        break;
                    case "xml":
                        try
                        {
                            StreamReader reader = new StreamReader(new FileStream(args[1], FileMode.Open));
                            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                            snapshot.LoadFromXml(reader);
                            int importedRecordsCount = Program.fileCabinetService.Restore(snapshot);
                            Console.WriteLine("{0} records were imported from {1}.", importedRecordsCount, args[1]);
                            reader.Close();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine("Import error: file {0} is not exist.", args[1]);
                        }

                        break;
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
            var totalRecordsCount = Program.fileCabinetService.GetStat().total;
            var deletedRecordsCount = Program.fileCabinetService.GetStat().deleted;
            Console.WriteLine($"Totally {totalRecordsCount} record(s). Need to delete {deletedRecordsCount} record(s).");
        }

        private static void Purge(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                var totalAmountOfRecords = Program.fileCabinetService.GetStat().total;
                Program.fileCabinetService.Purge();
                var purgedRecords = totalAmountOfRecords - Program.fileCabinetService.GetStat().total;
                Console.WriteLine($"Data file processing is completed: {purgedRecords} of {totalAmountOfRecords} were purged.");
            }
        }

        private static void Exit(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                (Program.fileCabinetService as FileCabinetFilesystemService).Dispose();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
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
            if (source is null || source.Length == 0)
            {
                return new Tuple<bool, string, char>(false, source, ' ');
            }

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
            if (favouriteSymbol == Program.fileCabinetService.Validator.ExcludeChar)
            {
                return new Tuple<bool, string>(false, favouriteSymbol.ToString());
            }

            return new Tuple<bool, string>(true, favouriteSymbol.ToString());
        }
    }
}
