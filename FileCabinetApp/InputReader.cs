using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// The InputReader class.
    /// </summary>
    public static class InputReader
    {
        /// <summary>
        /// Creates the record by all the fields.
        /// </summary>
        /// <returns>The created record.</returns>
        public static FileCabinetRecord CreateRecord()
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

            return new FileCabinetRecord { Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol };
        }

        /// <summary>
        /// Creates the record by recordFields with given recordValues.
        /// </summary>
        /// <param name="recordFields">The fields.</param>
        /// <param name="recordValues">The values.</param>
        /// <returns>The created record.</returns>
        public static FileCabinetRecord CreateRecord(string[] recordFields, string[] recordValues)
        {
            int id = ReadInput(recordFields, recordValues, IdSearcher, IntConverter, IdValidator);
            string firstName = ReadInput(recordFields, recordValues, FirstNameSearcher, StringConverter, FirstNameValidator);
            string lastName = ReadInput(recordFields, recordValues, LastNameSearcher, StringConverter, LastNameValidator);
            DateTime dateOfBirth = ReadInput(recordFields, recordValues, DateOfBirthSearcher, DateConverter, DateOfBirthValidator);
            short grade = ReadInput(recordFields, recordValues, GradeSearcher, ShortConverter, GradeValidator);
            decimal height = ReadInput(recordFields, recordValues, HeightSearcher, DecimalConverter, HeightValidator);
            char favouriteSymbol = ReadInput(recordFields, recordValues, FavouriteSymbolSearcher, CharConverter, FavouriteSymbolValidator);
            return new FileCabinetRecord { Id = id, Name = new Name { FirstName = firstName, LastName = lastName }, DateOfBirth = dateOfBirth, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol };
        }

        /// <summary>
        /// Updates the records recordFields with the given recordValues.
        /// </summary>
        /// <param name="recordFields">The fields to update.</param>
        /// <param name="recordValues">The values to update.</param>
        /// <param name="recordToUpdate">The record to update.</param>
        /// <returns>The updated record.</returns>
        public static FileCabinetRecord UpdateRecord(string[] recordFields, string[] recordValues, FileCabinetRecord recordToUpdate)
        {
            if (recordToUpdate is null)
            {
                throw new ArgumentNullException(nameof(recordToUpdate), "RecordToUpdate is null.");
            }

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

        private static Tuple<bool, string> IdSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("id"))
            {
                return new Tuple<bool, string>(false, "id is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "id");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> FirstNameSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("firstname"))
            {
                return new Tuple<bool, string>(false, "firstname is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "firstname");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> LastNameSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("lastname"))
            {
                return new Tuple<bool, string>(false, "lastname is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "lastname");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> DateOfBirthSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("dateofbirth"))
            {
                return new Tuple<bool, string>(false, "dateofbirth is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "dateofbirth");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> GradeSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("grade"))
            {
                return new Tuple<bool, string>(false, "grade is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "grade");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> HeightSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("height"))
            {
                return new Tuple<bool, string>(false, "height is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "height");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string> FavouriteSymbolSearcher(string[] recordFields, string[] recordValues)
        {
            if (!recordFields.Contains("favouritesymbol"))
            {
                return new Tuple<bool, string>(false, "favouritesymbol is not found");
            }

            int index = recordFields.ToList().FindIndex(x => x == "favouritesymbol");
            string value = recordValues[index];

            return new Tuple<bool, string>(true, value);
        }

        private static Tuple<bool, string, int> IntConverter(string source)
        {
            int id;
            bool idsuccess = int.TryParse(source, out id);

            return new Tuple<bool, string, int>(idsuccess, source, id);
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
            bool heightSuccess = decimal.TryParse(source, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out height);

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

        private static Tuple<bool, string> IdValidator(int id)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>(Program.FileCabinetService.GetRecords());
            var record = records.Find(record => record.Id == id);
            if (record != null || id < 1)
            {
                return new Tuple<bool, string>(false, $"Id should be more than 1 and unique");
            }

            return new Tuple<bool, string>(true, $"{id}");
        }

        private static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (firstName is null || firstName.Trim().Length == 0 || firstName.Length < Program.ValidationRules.FirstNameMinLengthInSymbols || firstName.Length > Program.ValidationRules.FirstNameMaxLengthInSymbols)
            {
                return new Tuple<bool, string>(false, $"Firstname length should be in range [{Program.ValidationRules.FirstNameMinLengthInSymbols};{Program.ValidationRules.FirstNameMaxLengthInSymbols}]");
            }

            return new Tuple<bool, string>(true, firstName);
        }

        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            if (lastName is null || lastName.Trim().Length == 0 || lastName.Length < Program.ValidationRules.LastNameMinLengthInSymbols || lastName.Length > Program.ValidationRules.LastNameMaxLengthInSymbols)
            {
                return new Tuple<bool, string>(false, $"Lastname length should be in range [{Program.ValidationRules.LastNameMinLengthInSymbols};{Program.ValidationRules.LastNameMaxLengthInSymbols}]");
            }

            return new Tuple<bool, string>(true, lastName);
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth < Program.ValidationRules.DateOfBirthMinimalDate || dateOfBirth > Program.ValidationRules.DateOfBirthMaximalDate)
            {
                return new Tuple<bool, string>(false, $"Date should start from {Program.ValidationRules.DateOfBirthMinimalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {Program.ValidationRules.DateOfBirthMaximalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}");
            }

            return new Tuple<bool, string>(true, dateOfBirth.ToString("MM'/'dd'/'yyyy", null));
        }

        private static Tuple<bool, string> GradeValidator(short grade)
        {
            if (grade < Program.ValidationRules.GradeMinValueInPoints || grade > Program.ValidationRules.GradeMaxValueInPoints)
            {
                return new Tuple<bool, string>(false, $"Grade should be in range [{Program.ValidationRules.GradeMinValueInPoints};{Program.ValidationRules.GradeMaxValueInPoints}]");
            }

            return new Tuple<bool, string>(true, $"{grade}");
        }

        private static Tuple<bool, string> HeightValidator(decimal height)
        {
            if (height < Program.ValidationRules.HeightMinValueInMeters || height > Program.ValidationRules.HeightMaxValueInMeters)
            {
                return new Tuple<bool, string>(false, $"Height can't be lower {Program.ValidationRules.HeightMinValueInMeters} and higher than {Program.ValidationRules.HeightMaxValueInMeters}");
            }

            return new Tuple<bool, string>(true, $"{height}");
        }

        private static Tuple<bool, string> FavouriteSymbolValidator(char favouriteSymbol)
        {
            if (favouriteSymbol == Program.ValidationRules.FavouriteSymbolBannedChar)
            {
                return new Tuple<bool, string>(false, $"{Program.ValidationRules.FavouriteSymbolBannedChar} is not valid symbol");
            }

            return new Tuple<bool, string>(true, $"{favouriteSymbol}");
        }
    }
}
