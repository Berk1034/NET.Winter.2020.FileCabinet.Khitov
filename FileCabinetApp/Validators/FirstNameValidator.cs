using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The FirstNameValidator class.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">The minimal length for validation.</param>
        /// <param name="maxLength">The maximal length for validation.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
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

            if (recordInfo.Name.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "Firstname can't be null.");
            }

            if (recordInfo.Name.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo));
            }

            if (recordInfo.Name.FirstName.Length < this.minLength || recordInfo.Name.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"Firstname length should be in range [{this.minLength};{this.maxLength}].", nameof(recordInfo));
            }
        }
    }
}
