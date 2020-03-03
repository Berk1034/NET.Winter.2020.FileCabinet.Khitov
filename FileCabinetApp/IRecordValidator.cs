using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The IRecordValidator interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Gets the value of MinLength.
        /// </summary>
        /// <value>
        /// The value of MinLength.
        /// </value>
        public int MinLength { get; }

        /// <summary>
        /// Gets the value of MaxLength.
        /// </summary>
        /// <value>
        /// The value of MaxLength.
        /// </value>
        public int MaxLength { get; }

        /// <summary>
        /// Gets the value of MinGrade.
        /// </summary>
        /// <value>
        /// The value of MinGrade.
        /// </value>
        public short MinGrade { get; }

        /// <summary>
        /// Gets the value of MaxGrade.
        /// </summary>
        /// <value>
        /// The value of MaxGrade.
        /// </value>
        public short MaxGrade { get; }

        /// <summary>
        /// Gets the value of MinHeight.
        /// </summary>
        /// <value>
        /// The value of MinHeight.
        /// </value>
        public decimal MinHeight { get; }

        /// <summary>
        /// Gets the value of MaxHeight.
        /// </summary>
        /// <value>
        /// The value of MaxHeight.
        /// </value>
        public decimal MaxHeight { get; }

        /// <summary>
        /// Gets the value of minimal DateTime.
        /// </summary>
        /// <value>
        /// The value of minimal DateTime.
        /// </value>
        public DateTime MinimalDate { get; }

        /// <summary>
        /// Gets the value of maximal DateTime.
        /// </summary>
        /// <value>
        /// The value of maximal DateTime.
        /// </value>
        public DateTime MaximalDate { get; }

        /// <summary>
        /// Gets the value of exclude char.
        /// </summary>
        /// <value>
        /// The value of exclude char.
        /// </value>
        public char ExcludeChar { get; }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="recordInfo">The record information for validation.</param>
        public void ValidateParameters(FileCabinetRecordInfo recordInfo);
    }
}
