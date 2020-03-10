using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetDefaultService class.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetMemoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base(new ValidatorBuilder()
                  .ValidateFirstName(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols)
                  .ValidateLastName(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols)
                  .ValidateDateOfBirth(ValidationRules.DefaultMinimalDate, ValidationRules.DefaultMaximalDate)
                  .ValidateGrade(ValidationRules.DefaultMinGradeInPoints, ValidationRules.DefaultMaxGradeInPoints)
                  .ValidateHeight(ValidationRules.DefaultMinHeightInMeters, ValidationRules.DefaultMaxHeightInMeters)
                  .ValidateFavouriteSymbol(ValidationRules.DefaultBannedChar)
                  .Create())
        {
        }
    }
}
