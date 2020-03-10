using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The CreateCommandHandler class.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "create")
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
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
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
