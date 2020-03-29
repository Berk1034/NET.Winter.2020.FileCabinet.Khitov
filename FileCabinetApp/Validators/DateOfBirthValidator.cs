using System;
using System.Globalization;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The DateOfBirthValidator class.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">The date from for validation.</param>
        /// <param name="to">The date to for validation.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates the record information.
        /// </summary>
        /// <param name="recordInfo">The record informaton.</param>
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "RecordInfo is null.");
            }

            if (recordInfo.DateOfBirth < this.from || recordInfo.DateOfBirth > this.to)
            {
                throw new ArgumentException($"Date should start from {this.from.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {this.to.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo));
            }
        }
    }
}
