using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The LastNameValidator class.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">The minimal length for validation.</param>
        /// <param name="maxLength">The maximal length for validation.</param>
        public LastNameValidator(int minLength, int maxLength)
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
            if (recordInfo.Name.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.Name.FirstName), "Lastname can't be null.");
            }

            if (recordInfo.Name.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Lastname cannot contain only spaces.", nameof(recordInfo.Name.FirstName));
            }

            if (recordInfo.Name.FirstName.Length < this.minLength || recordInfo.Name.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"Lastname length should be in range [{this.minLength};{this.maxLength}].", nameof(recordInfo.Name.FirstName));
            }
        }
    }
}
