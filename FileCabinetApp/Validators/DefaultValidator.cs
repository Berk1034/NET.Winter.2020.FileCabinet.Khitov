using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The DefaultValidator class.
    /// </summary>
    public class DefaultValidator : IRecordValidator
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

            new DefaultFirstNameValidator().ValidateParameters(recordInfo);
            new DefaultLastNameValidator().ValidateParameters(recordInfo);
            new DefaultDateOfBirthValidator().ValidateParameters(recordInfo);
            new DefaultGradeValidator().ValidateParameters(recordInfo);
            new DefaultHeightValidator().ValidateParameters(recordInfo);
            new DefaultFavouriteSymbolValidator().ValidateParameters(recordInfo);
        }
    }
}
