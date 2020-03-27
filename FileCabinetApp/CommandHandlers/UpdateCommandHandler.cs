using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The UpdateCommandHandler class.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private ValidationRules validationRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="validationRules">The validation rules.</param>
        public UpdateCommandHandler(IFileCabinetService service, ValidationRules validationRules)
            : base(service)
        {
            this.validationRules = validationRules;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest), "AppCommandRequest is null.");
            }

            if (appCommandRequest.Command == "update")
            {
                try
                {
                    char[] separators = { '=', ' ', ',' };
                    string[] sourceInput = appCommandRequest.Parameters.Split("where");

                    if (sourceInput.Length == 2)
                    {
                        string[] recordFieldsAndValues = sourceInput[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        string[] conditions = sourceInput[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        List<string> recordFields = new List<string>();
                        List<string> recordValues = new List<string>();

                        if (recordFieldsAndValues.Length < 3)
                        {
                            throw new ArgumentException("Not enough parameters after 'update' command.");
                        }

                        if (recordFieldsAndValues.Length > 2 && !recordFieldsAndValues.Contains("set"))
                        {
                            throw new ArgumentException("No separator 'set' after 'update' command!");
                        }

                        if (conditions.Length < 2)
                        {
                            throw new ArgumentException("Not enough parameters after 'where' condition.");
                        }

                        if (conditions.Length > 2 && !conditions.Contains("and") && !conditions.Contains("or"))
                        {
                            throw new ArgumentException("No separators 'and/or' after parameters!");
                        }

                        if (conditions.Contains("and") && conditions.Contains("or"))
                        {
                            throw new ArgumentException("Can't contain both 'and/or' separators.");
                        }

                        for (int i = 1; i < recordFieldsAndValues.Length; i += 2)
                        {
                            recordFieldsAndValues[i] = recordFieldsAndValues[i].ToLower(null);
                            recordFields.Add(recordFieldsAndValues[i]);
                        }

                        for (int i = 2; i < recordFieldsAndValues.Length; i += 2)
                        {
                            if (recordFieldsAndValues[i][0] == '\'')
                            {
                                recordFieldsAndValues[i] = recordFieldsAndValues[i].Substring(1, recordFieldsAndValues[i].Length - 2);
                            }

                            recordValues.Add(recordFieldsAndValues[i]);
                        }

                        for (int i = 0; i < conditions.Length; i += 3)
                        {
                            conditions[i] = conditions[i].ToLower(null);
                        }

                        for (int i = 1; i < conditions.Length; i += 3)
                        {
                            if (conditions[i][0] == '\'')
                            {
                                conditions[i] = conditions[i].Substring(1, conditions[i].Length - 2);
                            }
                        }

                        if (recordFields.Contains("id"))
                        {
                            throw new ArgumentException("Can't update id.");
                        }

                        List<FileCabinetRecord> recordsMatch = new List<FileCabinetRecord>(this.Service.GetRecords());

                        string condition = string.Empty;
                        if (conditions.Contains("and"))
                        {
                            condition = "and";
                        }

                        if (conditions.Contains("or"))
                        {
                            condition = "or";
                        }

                        if (string.IsNullOrEmpty(condition))
                        {
                            for (int i = 0; i < conditions.Length; i += 3)
                            {
                                switch (conditions[i])
                                {
                                    case "id":
                                        int id;
                                        bool parseSuccess = int.TryParse(conditions[i + 1], out id);
                                        if (parseSuccess)
                                        {
                                            recordsMatch = recordsMatch.FindAll(record => { return record.Id == id; });
                                        }

                                        break;
                                    case "firstname":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByFirstName(conditions[i + 1]));

                                        break;
                                    case "lastname":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByLastName(conditions[i + 1]));

                                        break;
                                    case "dateofbirth":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByLastName(conditions[i + 1]));

                                        break;
                                }
                            }

                            Memoizer.Clear();
                        }
                        else
                        {
                            Func<FileCabinetRecord, bool> predicate1 = null;
                            Func<FileCabinetRecord, bool> predicate2 = null;

                            if (condition == "and")
                            {
                                this.AssignPredicates(out predicate1, out predicate2, conditions);
                                recordsMatch = recordsMatch.FindAll(record => { return predicate1(record) && predicate2(record); });
                            }
                            else if (condition == "or")
                            {
                                this.AssignPredicates(out predicate1, out predicate2, conditions);
                                recordsMatch = recordsMatch.FindAll(record => { return predicate1(record) || predicate2(record); });
                            }
                        }

                        var updatedIds = new List<int>();
                        StringBuilder updatedRecords = new StringBuilder();

                        for (int i = 0; i < recordsMatch.Count; i++)
                        {
                            var updatedRecord = this.UpdatedRecord(recordFields.ToArray(), recordValues.ToArray(), recordsMatch[i]);
                            updatedRecord.Id = recordsMatch[i].Id;

                            this.Service.EditRecord(updatedRecord);
                            updatedIds.Add(updatedRecord.Id);
                        }

                        if (updatedIds.Count != 0)
                        {
                            for (int i = 0; i < updatedIds.Count; i++)
                            {
                                if (i == (updatedIds.Count - 1))
                                {
                                    updatedRecords.Append($"#{updatedIds[i]}");
                                }
                                else
                                {
                                    updatedRecords.Append($"#{updatedIds[i]}, ");
                                }
                            }

                            if (updatedIds.Count == 1)
                            {
                                Console.WriteLine($"Record {updatedRecords.ToString()} is updated.");
                            }
                            else
                            {
                                Console.WriteLine($"Records {updatedRecords.ToString()} are updated.");
                            }

                            Memoizer.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"No records with such criteria found.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't update records.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }

        private void AssignPredicates(out Func<FileCabinetRecord, bool> firstPredicate, out Func<FileCabinetRecord, bool> secondPredicate, string[] conditions)
        {
            Func<FileCabinetRecord, bool> predicate1 = null;
            Func<FileCabinetRecord, bool> predicate2 = null;

            for (int i = 0; i < conditions.Length; i += 3)
            {
                switch (conditions[i])
                {
                    case "id":
                        int id;
                        bool parseSuccess = int.TryParse(conditions[i + 1], out id);
                        if (parseSuccess)
                        {
                            if (predicate1 == null)
                            {
                                predicate1 = record => record.Id == id;
                            }
                            else
                            {
                                predicate2 = record => record.Id == id;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Can't extract id. You typed invalid symbols!");
                        }

                        continue;
                    case "firstname":
                        string firstname = conditions[i + 1];
                        if (predicate1 == null)
                        {
                            predicate1 = record => record.Name.FirstName == firstname;
                        }
                        else
                        {
                            predicate2 = record => record.Name.FirstName == firstname;
                        }

                        continue;
                    case "lastname":
                        string lastname = conditions[i + 1];
                        if (predicate1 == null)
                        {
                            predicate1 = record => record.Name.LastName == lastname;
                        }
                        else
                        {
                            predicate2 = record => record.Name.LastName == lastname;
                        }

                        continue;
                    case "dateofbirth":
                        DateTime dateOfBirth;
                        bool dateSuccess = DateTime.TryParseExact(conditions[i + 1], "MM'/'dd'/'yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateOfBirth);
                        if (dateSuccess)
                        {
                            if (predicate1 == null)
                            {
                                predicate1 = record => record.DateOfBirth == dateOfBirth;
                            }
                            else
                            {
                                predicate2 = record => record.DateOfBirth == dateOfBirth;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Can't extract dateofbirth. You typed invalid symbols!");
                        }

                        continue;
                }
            }

            firstPredicate = predicate1;
            secondPredicate = predicate2;
        }

        private FileCabinetRecord UpdatedRecord(string[] recordFields, string[] recordValues, FileCabinetRecord recordToUpdate)
        {
            string firstName = ReadInput(recordFields, recordValues, recordToUpdate.Name.FirstName, FirstNameSearcher, StringConverter, FirstNameValidator);
            string lastName = ReadInput(recordFields, recordValues, recordToUpdate.Name.LastName, LastNameSearcher, StringConverter, LastNameValidator);
            DateTime dateofbirth = ReadInput(recordFields, recordValues, recordToUpdate.DateOfBirth, DateOfBirthSearcher, DateConverter, DateOfBirthValidator);
            short grade = ReadInput(recordFields, recordValues, recordToUpdate.Grade, GradeSearcher, ShortConverter, GradeValidator);
            decimal height = ReadInput(recordFields, recordValues, recordToUpdate.Height, HeightSearcher, DecimalConverter, HeightValidator);
            char favouriteSymbol = ReadInput(recordFields, recordValues, recordToUpdate.FavouriteSymbol, FavouriteSymbolSearcher, CharConverter, FavouriteSymbolValidator);

            return new FileCabinetRecord { Name = new Name { FirstName = firstName, LastName = lastName }, DateOfBirth = dateofbirth, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol };
        }

        private static T ReadInput<T>(string[] recordFields, string[] recordValues, T recordField, Func<string[], string[], Tuple<bool, string>> searcher, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            T value;

            var searchResult = searcher(recordFields, recordValues);
            if (!searchResult.Item1)
            {
                return recordField;
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
