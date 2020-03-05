using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The CustomValidator class.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private const int MinLengthInSymbols = 2;
        private const int MaxLengthInSymbols = 40;
        private const short MinGradeInPoints = 0;
        private const short MaxGradeInPoints = 5;
        private const decimal MinHeightInMeters = 0.4m;
        private const decimal MaxHeightInMeters = 3m;
        private const char BannedChar = ' ';

        /// <summary>
        /// Gets the value of MinLengthInSymbols const.
        /// </summary>
        /// <value>
        /// The value of MinLengthInSymbols const.
        /// </value>
        public int MinLength => MinLengthInSymbols;

        /// <summary>
        /// Gets the value of MaxLengthInSymbols const.
        /// </summary>
        /// <value>
        /// The value of MaxLengthInSymbols const.
        /// </value>
        public int MaxLength => MaxLengthInSymbols;

        /// <summary>
        /// Gets the value of MinGradeInPoints const.
        /// </summary>
        /// <value>
        /// The value of MinGradeInPoints const.
        /// </value>
        public short MinGrade => MinGradeInPoints;

        /// <summary>
        /// Gets the value of MaxGradeInPoints const.
        /// </summary>
        /// <value>
        /// The value of MaxGradeInPoints const.
        /// </value>
        public short MaxGrade => MaxGradeInPoints;

        /// <summary>
        /// Gets the value of MinHeight const.
        /// </summary>
        /// <value>
        /// The value of MinHeight const.
        /// </value>
        public decimal MinHeight => MinHeightInMeters;

        /// <summary>
        /// Gets the value of MaxHeight const.
        /// </summary>
        /// <value>
        /// The value of MaxHeight const.
        /// </value>
        public decimal MaxHeight => MaxHeightInMeters;

        /// <summary>
        /// Gets the value of minimal DateTime.
        /// </summary>
        /// <value>
        /// The value of minimal DateTime.
        /// </value>
        public DateTime MinimalDate => new DateTime(1960, 1, 1);

        /// <summary>
        /// Gets the value of DateTime.Now.
        /// </summary>
        /// <value>
        /// The value of DateTime.Now.
        /// </value>
        public DateTime MaximalDate => DateTime.Now;

        /// <summary>
        /// Gets the value of BannedChar const.
        /// </summary>
        /// <value>
        /// The value of BannedChar const.
        /// </value>
        public char ExcludeChar => BannedChar;

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

            if (recordInfo.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.FirstName), "Firstname can't be null.");
            }

            if (recordInfo.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo.FirstName));
            }

            if (recordInfo.FirstName.Length < MinLengthInSymbols || recordInfo.FirstName.Length > MaxLengthInSymbols)
            {
                throw new ArgumentException($"Firstname length should be in range [{MinLengthInSymbols};{MaxLengthInSymbols}].", nameof(recordInfo.FirstName));
            }

            if (recordInfo.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.LastName), "Lastname can't be null.");
            }

            if (recordInfo.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Lastname cannot contain only spaces.", nameof(recordInfo.LastName));
            }

            if (recordInfo.LastName.Length < MinLengthInSymbols || recordInfo.LastName.Length > MaxLengthInSymbols)
            {
                throw new ArgumentException($"Lastname length should be in range [{MinLengthInSymbols};{MaxLengthInSymbols}.", nameof(recordInfo.LastName));
            }

            if (recordInfo.DateOfBirth < this.MinimalDate || recordInfo.DateOfBirth > this.MaximalDate)
            {
                throw new ArgumentException($"Date should start from {this.MinimalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {this.MaximalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo.DateOfBirth));
            }

            if (recordInfo.Grade < MinGradeInPoints || recordInfo.Grade > MaxGradeInPoints)
            {
                throw new ArgumentException($"Grade should be in range [{MinGradeInPoints};{MaxGradeInPoints}].", nameof(recordInfo.Grade));
            }

            if (recordInfo.Height < MinHeightInMeters || recordInfo.Height > MaxHeightInMeters)
            {
                throw new ArgumentException($"Height can't be lower {MinHeightInMeters} and higher than {MaxHeightInMeters}.", nameof(recordInfo.Height));
            }

            if (recordInfo.FavouriteSymbol == BannedChar)
            {
                throw new ArgumentException($"{BannedChar} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
