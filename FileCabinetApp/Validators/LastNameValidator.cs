using System;

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
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "RecordInfo is null.");
            }

            if (recordInfo.Name.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "Lastname can't be null.");
            }

            if (recordInfo.Name.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Lastname cannot contain only spaces.", nameof(recordInfo));
            }

            if (recordInfo.Name.LastName.Length < this.minLength || recordInfo.Name.LastName.Length > this.maxLength)
            {
                throw new ArgumentException($"Lastname length should be in range [{this.minLength};{this.maxLength}].", nameof(recordInfo));
            }
        }
    }
}
