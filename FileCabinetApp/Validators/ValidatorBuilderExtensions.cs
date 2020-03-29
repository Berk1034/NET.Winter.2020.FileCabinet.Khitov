using System;
using System.IO;
using Microsoft.Extensions.Configuration;

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
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), "ValidatorBuilder is null");
            }

            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var builder = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("validation-rules.json");
            var config = builder.Build();
            var validationConfig = config.GetSection("default").Get<ValidationRules>();

            return validatorBuilder
                .ValidateFirstName(validationConfig.FirstNameMinLengthInSymbols, validationConfig.FirstNameMaxLengthInSymbols)
                .ValidateLastName(validationConfig.LastNameMinLengthInSymbols, validationConfig.LastNameMaxLengthInSymbols)
                .ValidateDateOfBirth(validationConfig.DateOfBirthMinimalDate, validationConfig.DateOfBirthMaximalDate)
                .ValidateGrade(validationConfig.GradeMinValueInPoints, validationConfig.GradeMaxValueInPoints)
                .ValidateHeight(validationConfig.HeightMinValueInMeters, validationConfig.HeightMaxValueInMeters)
                .ValidateFavouriteSymbol(validationConfig.FavouriteSymbolBannedChar)
                .Create();
        }

        /// <summary>
        /// The extension method for ValidatorBuilder class to create a CustomValidator.
        /// </summary>
        /// <param name="validatorBuilder">The Validator builder instance.</param>
        /// <returns>The IRecordValidator reference.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), "ValidatorBuilder is null");
            }

            string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var builder = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("validation-rules.json");
            var config = builder.Build();
            var validationConfig = config.GetSection("custom").Get<ValidationRules>();

            return validatorBuilder
                .ValidateFirstName(validationConfig.FirstNameMinLengthInSymbols, validationConfig.FirstNameMaxLengthInSymbols)
                .ValidateLastName(validationConfig.LastNameMinLengthInSymbols, validationConfig.LastNameMaxLengthInSymbols)
                .ValidateDateOfBirth(validationConfig.DateOfBirthMinimalDate, validationConfig.DateOfBirthMaximalDate)
                .ValidateGrade(validationConfig.GradeMinValueInPoints, validationConfig.GradeMaxValueInPoints)
                .ValidateHeight(validationConfig.HeightMinValueInMeters, validationConfig.HeightMaxValueInMeters)
                .ValidateFavouriteSymbol(validationConfig.FavouriteSymbolBannedChar)
                .Create();
        }
    }
}
