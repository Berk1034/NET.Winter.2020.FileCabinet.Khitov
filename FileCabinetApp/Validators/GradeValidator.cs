using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The GradeValidator class.
    /// </summary>
    public class GradeValidator : IRecordValidator
    {
        private short minGrade;
        private short maxGrade;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeValidator"/> class.
        /// </summary>
        /// <param name="minGrade">The minimal grade for validation.</param>
        /// <param name="maxGrade">The maximal grade for validation.</param>
        public GradeValidator(short minGrade, short maxGrade)
        {
            this.minGrade = minGrade;
            this.maxGrade = maxGrade;
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

            if (recordInfo.Grade < this.minGrade || recordInfo.Grade > this.maxGrade)
            {
                throw new ArgumentException($"Grade should be in range [{this.minGrade};{this.maxGrade}].", nameof(recordInfo));
            }
        }
    }
}
