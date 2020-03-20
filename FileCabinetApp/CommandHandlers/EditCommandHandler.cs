using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The EditCommandHandler class.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private ValidationRules validationRules;
        private ServiceMeter serviceMeter;
        private ServiceLogger serviceLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="validationRules">The validation rules.</param>
        /// <param name="serviceMeter">The service meter to measure execution time of service methods.</param>
        /// <param name="serviceLogger">The service logger to log every method call of service methods.</param>
        public EditCommandHandler(IFileCabinetService service, ValidationRules validationRules, ServiceMeter serviceMeter, ServiceLogger serviceLogger)
            : base(service)
        {
            this.validationRules = validationRules;
            this.serviceMeter = serviceMeter;
            this.serviceLogger = serviceLogger;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "edit")
            {
                var listOfRecords = new List<FileCabinetRecord>(this.service.GetRecords());
                int editId;
                bool parseSuccess = int.TryParse(appCommandRequest.Parameters, out editId);
                if (!parseSuccess)
                {
                    Console.WriteLine("You typed invalid symbols!");
                }
                else
                {
                    int index = listOfRecords.FindIndex((record) => record.Id == editId);
                    if (index == -1)
                    {
                        Console.WriteLine($"#{appCommandRequest.Parameters} record is not found.");
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

                        if (this.serviceLogger != null)
                        {
                            this.serviceLogger.EditRecord(new FileCabinetRecord { Id = listOfRecords[index].Id, Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                        }
                        else if (this.serviceMeter != null)
                        {
                            this.serviceMeter.EditRecord(new FileCabinetRecord { Id = listOfRecords[index].Id, Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                        }
                        else
                        {
                            this.service.EditRecord(new FileCabinetRecord { Id = listOfRecords[index].Id, Name = new Name { FirstName = name, LastName = surname }, DateOfBirth = birthday, Grade = grade, Height = height, FavouriteSymbol = favouriteSymbol });
                        }

                        Console.WriteLine($"Record #{appCommandRequest.Parameters} is updated.");
                    }
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }

        private T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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
