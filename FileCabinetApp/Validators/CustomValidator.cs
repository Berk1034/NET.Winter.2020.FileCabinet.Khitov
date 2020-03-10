using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The CustomValidator class.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the record information.
        /// </summary>
        /// <param name="recordInfo">The record informaton.</param>
        /// <exception cref="ArgumentException()">Thrown when no record with such id found or when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "Record information is null.");
            }

            this.ValidateFirstName(recordInfo);
            this.ValidateLastName(recordInfo);
            this.ValidateDateOfBirth(recordInfo);
            this.ValidateGrade(recordInfo);
            this.ValidateHeight(recordInfo);
            this.ValidateFavouriteSymbol(recordInfo);
        }

        private void ValidateFirstName(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Name.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.Name.FirstName), "Firstname can't be null.");
            }

            if (recordInfo.Name.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo.Name.FirstName));
            }

            if (recordInfo.Name.FirstName.Length < ValidationRules.CustomMinLengthInSymbols || recordInfo.Name.FirstName.Length > ValidationRules.CustomMaxLengthInSymbols)
            {
                throw new ArgumentException($"Firstname length should be in range [{ValidationRules.CustomMinLengthInSymbols};{ValidationRules.CustomMaxLengthInSymbols}].", nameof(recordInfo.Name.FirstName));
            }
        }

        private void ValidateLastName(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Name.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.Name.LastName), "Lastname can't be null.");
            }

            if (recordInfo.Name.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Lastname cannot contain only spaces.", nameof(recordInfo.Name.LastName));
            }

            if (recordInfo.Name.LastName.Length < ValidationRules.CustomMinLengthInSymbols || recordInfo.Name.LastName.Length > ValidationRules.CustomMaxLengthInSymbols)
            {
                throw new ArgumentException($"Lastname length should be in range [{ValidationRules.CustomMinLengthInSymbols};{ValidationRules.CustomMaxLengthInSymbols}.", nameof(recordInfo.Name.LastName));
            }
        }

        private void ValidateDateOfBirth(FileCabinetRecord recordInfo)
        {
            if (recordInfo.DateOfBirth < ValidationRules.CustomMinimalDate || recordInfo.DateOfBirth > ValidationRules.CustomMaximalDate)
            {
                throw new ArgumentException($"Date should start from {ValidationRules.CustomMinimalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {ValidationRules.CustomMaximalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo.DateOfBirth));
            }
        }

        private void ValidateGrade(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Grade < ValidationRules.CustomMinGradeInPoints || recordInfo.Grade > ValidationRules.CustomMaxGradeInPoints)
            {
                throw new ArgumentException($"Grade should be in range [{ValidationRules.CustomMinGradeInPoints};{ValidationRules.CustomMaxGradeInPoints}].", nameof(recordInfo.Grade));
            }
        }

        private void ValidateHeight(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Height < ValidationRules.CustomMinHeightInMeters || recordInfo.Height > ValidationRules.CustomMaxHeightInMeters)
            {
                throw new ArgumentException($"Height can't be lower {ValidationRules.CustomMinHeightInMeters} and higher than {ValidationRules.CustomMaxHeightInMeters}.", nameof(recordInfo.Height));
            }
        }

        private void ValidateFavouriteSymbol(FileCabinetRecord recordInfo)
        {
            if (recordInfo.FavouriteSymbol == ValidationRules.CustomBannedChar)
            {
                throw new ArgumentException($"{ValidationRules.CustomBannedChar} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
