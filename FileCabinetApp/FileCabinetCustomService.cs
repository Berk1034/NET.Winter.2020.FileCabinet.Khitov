using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetCustomService class.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetMemoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        public FileCabinetCustomService()
            : base(new ValidatorBuilder()
                  .ValidateFirstName(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols)
                  .ValidateLastName(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols)
                  .ValidateDateOfBirth(ValidationRules.CustomMinimalDate, ValidationRules.CustomMaximalDate)
                  .ValidateGrade(ValidationRules.CustomMinGradeInPoints, ValidationRules.CustomMaxGradeInPoints)
                  .ValidateHeight(ValidationRules.CustomMinHeightInMeters, ValidationRules.CustomMaxHeightInMeters)
                  .ValidateFavouriteSymbol(ValidationRules.CustomBannedChar)
                  .Create())
        {
        }
    }
}
