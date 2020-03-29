using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The HeightValidator class.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private decimal minHeight;
        private decimal maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">The minimal height for validation.</param>
        /// <param name="maxHeight">The maximal height for validation.</param>
        public HeightValidator(decimal minHeight, decimal maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
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

            if (recordInfo.Height < this.minHeight || recordInfo.Height > this.maxHeight)
            {
                throw new ArgumentException($"Height can't be lower {this.minHeight} and higher than {this.maxHeight}.", nameof(recordInfo));
            }
        }
    }
}
