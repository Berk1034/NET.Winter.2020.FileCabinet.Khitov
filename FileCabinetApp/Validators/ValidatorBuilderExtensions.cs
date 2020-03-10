using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The ValidatorBuilderExtensions class.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// The extension method for ValidatorBuilder class to create a DefaultValidator.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder instance.</param>
        /// <returns>The IRecordValidator reference.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            return validatorBuilder
                .ValidateFirstName(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols)
                .ValidateLastName(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols)
                .ValidateDateOfBirth(ValidationRules.DefaultMinimalDate, ValidationRules.DefaultMaximalDate)
                .ValidateGrade(ValidationRules.DefaultMinGradeInPoints, ValidationRules.DefaultMaxGradeInPoints)
                .ValidateHeight(ValidationRules.DefaultMinHeightInMeters, ValidationRules.DefaultMaxHeightInMeters)
                .ValidateFavouriteSymbol(ValidationRules.DefaultBannedChar)
                .Create();
        }

        /// <summary>
        /// The extension method for ValidatorBuilder class to create a CustomValidator.
        /// </summary>
        /// <param name="validatorBuilder">The Validator builder instance.</param>
        /// <returns>The IRecordValidator reference.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            return validatorBuilder
                .ValidateFirstName(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols)
                .ValidateLastName(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols)
                .ValidateDateOfBirth(ValidationRules.CustomMinimalDate, ValidationRules.CustomMaximalDate)
                .ValidateGrade(ValidationRules.CustomMinGradeInPoints, ValidationRules.CustomMaxGradeInPoints)
                .ValidateHeight(ValidationRules.CustomMinHeightInMeters, ValidationRules.CustomMaxHeightInMeters)
                .ValidateFavouriteSymbol(ValidationRules.CustomBannedChar)
                .Create();
        }
    }
}
