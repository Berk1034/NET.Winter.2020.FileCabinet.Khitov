using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The InsertCommandHandler class.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private ValidationRules validationRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="validationRules">The validation rules.</param>
        public InsertCommandHandler(IFileCabinetService service, ValidationRules validationRules)
            : base(service)
        {
            this.service = service;
            this.validationRules = validationRules;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "insert")
            {
                try
                {
                    char[] separators = { '(', ')', ',', ' ' };
                    string[] sourceInput = appCommandRequest.Parameters.Split("values");

                    if (sourceInput.Length == 2)
                    {
                        string[] recordFields = sourceInput[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        string[] recordValues = sourceInput[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        if (recordFields.Length == recordValues.Length)
                        {
                            for (int i = 0; i < recordFields.Length; i++)
                            {
                                recordFields[i] = recordFields[i].ToLower(null);
                            }

                            for (int i = 0; i < recordValues.Length; i++)
                            {
                                if (recordValues[i][0] == '\'')
                                {
                                    recordValues[i] = recordValues[i].Substring(1, recordValues[i].Length - 2);
                                }
                            }

                            int id = ReadInput(recordFields, recordValues, IdSearcher, IntConverter, IdValidator);
                            string firstName = ReadInput(recordFields, recordValues, FirstNameSearcher, StringConverter, FirstNameValidator);
                            string lastName = ReadInput(recordFields, recordValues, LastNameSearcher, StringConverter, LastNameValidator);
                            DateTime dateOfBirth = ReadInput(recordFields, recordValues, DateOfBirthSearcher, DateConverter, DateOfBirthValidator);
                            short grade = ReadInput(recordFields, recordValues, GradeSearcher, ShortConverter, GradeValidator);
                            decimal height = ReadInput(recordFields, recordValues, HeightSearcher, DecimalConverter, HeightValidator);
                            char favouriteSymbol = ReadInput(recordFields, recordValues, FavouriteSymbolSearcher, CharConverter, FavouriteSymbolValidator);
                            var recordId = this.service.CreateRecord(new FileCabinetRecord { Id = id, Name = new Name { FirstName = firstName, LastName = lastName }, DateOfBirth = dateOfBirth, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });

                            Memoizer.Clear();

                            Console.WriteLine($"Record #{recordId} is created.");
                        }
                        else
                        {
                            Console.WriteLine("Can't insert record.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't insert record.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }

        private static T ReadInput<T>(string[] recordFields, string[] recordValues, Func<string[], string[], Tuple<bool, string>> searcher, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            T value;

            var searchResult = searcher(recordFields, recordValues);
            if (!searchResult.Item1)
            {
                throw new ArgumentException($"Search failed: {searchResult.Item2}. Please, correct your input.");
            }

            var input = searchResult.Item2;

            var conversionResult = converter(input);
            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            value = conversionResult.Item3;
            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private Tuple<bool, string> IdSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("id"))
            {
                return new Tuple<bool, string>(false, "id is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "id");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> FirstNameSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("firstname"))
            {
                return new Tuple<bool, string>(false, "firstname is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "firstname");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> LastNameSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("lastname"))
            {
                return new Tuple<bool, string>(false, "lastname is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "lastname");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> DateOfBirthSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("dateofbirth"))
            {
                return new Tuple<bool, string>(false, "dateofbirth is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "dateofbirth");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> GradeSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("grade"))
            {
                return new Tuple<bool, string>(false, "grade is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "grade");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> HeightSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("height"))
            {
                return new Tuple<bool, string>(false, "height is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "height");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string> FavouriteSymbolSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("favouritesymbol"))
            {
                return new Tuple<bool, string>(false, "favouritesymbol is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "favouritesymbol");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private Tuple<bool, string, int> IntConverter(string source)
        {
            int id;
            bool idsuccess = int.TryParse(source, out id);

            return new Tuple<bool, string, int>(idsuccess, source, id);
        }

        private Tuple<bool, string, string> StringConverter(string source)
        {
            return new Tuple<bool, string, string>(true, source, source);
        }

        private Tuple<bool, string, DateTime> DateConverter(string dateOfBirth)
        {
            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "MM'/'dd'/'yyyy", null, DateTimeStyles.None, out birthday);

            return new Tuple<bool, string, DateTime>(dateSuccess, dateOfBirth, birthday);
        }

        private Tuple<bool, string, short> ShortConverter(string source)
        {
            short grade;
            bool gradeSuccess = short.TryParse(source, out grade);

            return new Tuple<bool, string, short>(gradeSuccess, source, grade);
        }

        private Tuple<bool, string, decimal> DecimalConverter(string source)
        {
            decimal height;
            bool heightSuccess = decimal.TryParse(source, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out height);

            return new Tuple<bool, string, decimal>(heightSuccess, source, height);
        }

        private Tuple<bool, string, char> CharConverter(string source)
        {
            if (source is null || source.Length == 0)
            {
                return new Tuple<bool, string, char>(false, source, ' ');
            }

            return new Tuple<bool, string, char>(true, source, source[0]);
        }

        private Tuple<bool, string> IdValidator(int id)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>(this.service.GetRecords());
            var record = records.Find(record => record.Id == id);
            if (record != null || id < 1)
            {
                return new Tuple<bool, string>(false, id.ToString());
            }

            return new Tuple<bool, string>(true, id.ToString());
        }

        private Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (firstName is null || firstName.Trim().Length == 0 || firstName.Length < this.validationRules.FirstNameMinLengthInSymbols || firstName.Length > this.validationRules.FirstNameMaxLengthInSymbols)
            {
                return new Tuple<bool, string>(false, firstName);
            }

            return new Tuple<bool, string>(true, firstName);
        }

        private Tuple<bool, string> LastNameValidator(string lastName)
        {
            if (lastName is null || lastName.Trim().Length == 0 || lastName.Length < this.validationRules.LastNameMinLengthInSymbols || lastName.Length > this.validationRules.LastNameMaxLengthInSymbols)
            {
                return new Tuple<bool, string>(false, lastName);
            }

            return new Tuple<bool, string>(true, lastName);
        }

        private Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth < this.validationRules.DateOfBirthMinimalDate || dateOfBirth > this.validationRules.DateOfBirthMaximalDate)
            {
                return new Tuple<bool, string>(false, dateOfBirth.ToString("MM'/'dd'/'yyyy", null));
            }

            return new Tuple<bool, string>(true, dateOfBirth.ToString("MM'/'dd'/'yyyy", null));
        }

        private Tuple<bool, string> GradeValidator(short grade)
        {
            if (grade < this.validationRules.GradeMinValueInPoints || grade > this.validationRules.GradeMaxValueInPoints)
            {
                return new Tuple<bool, string>(false, grade.ToString());
            }

            return new Tuple<bool, string>(true, grade.ToString());
        }

        private Tuple<bool, string> HeightValidator(decimal height)
        {
            if (height < this.validationRules.HeightMinValueInMeters || height > this.validationRules.HeightMaxValueInMeters)
            {
                return new Tuple<bool, string>(false, height.ToString());
            }

            return new Tuple<bool, string>(true, height.ToString());
        }

        private Tuple<bool, string> FavouriteSymbolValidator(char favouriteSymbol)
        {
            if (favouriteSymbol == this.validationRules.FavouriteSymbolBannedChar)
            {
                return new Tuple<bool, string>(false, favouriteSymbol.ToString());
            }

            return new Tuple<bool, string>(true, favouriteSymbol.ToString());
        }

    }
}
