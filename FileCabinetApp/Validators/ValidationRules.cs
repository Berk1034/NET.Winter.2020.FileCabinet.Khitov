using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The ValidationRules class.
    /// </summary>
    public class ValidationRules
    {
        /// <summary>
        /// Gets or sets the minimal length for the record first name.
        /// </summary>
        /// <value>
        /// The minimal length for the record first name.
        /// </value>
        public int FirstNameMinLengthInSymbols { get; set; }

        /// <summary>
        /// Gets or sets the maximal length for the record first name.
        /// </summary>
        /// <value>
        /// The maximal length for the record first name.
        /// </value>
        public int FirstNameMaxLengthInSymbols { get; set; }

        /// <summary>
        /// Gets or sets the minimal length for the record last name.
        /// </summary>
        /// <value>
        /// The minimal length for the record last name.
        /// </value>
        public int LastNameMinLengthInSymbols { get; set; }

        /// <summary>
        /// Gets or sets the maximal length for the record last name.
        /// </summary>
        /// <value>
        /// The maximal length for the record last name.
        /// </value>
        public int LastNameMaxLengthInSymbols { get; set; }

        /// <summary>
        /// Gets or sets the minimal datetime for the record date of birth.
        /// </summary>
        /// <value>
        /// The minimal datetime for the record date of birth.
        /// </value>
        public DateTime DateOfBirthMinimalDate { get; set; }

        /// <summary>
        /// Gets or sets the maximal datetime for the record date of birth.
        /// </summary>
        /// <value>
        /// The maximal datetime for the record date of birth.
        /// </value>
        public DateTime DateOfBirthMaximalDate { get; set; }

        /// <summary>
        /// Gets or sets the minimal value for the record grade.
        /// </summary>
        /// <value>
        /// The minimal value for the record grade.
        /// </value>
        public short GradeMinValueInPoints { get; set; }

        /// <summary>
        /// Gets or sets the maximal value for the record grade.
        /// </summary>
        /// <value>
        /// The maximal value for the record grade.
        /// </value>
        public short GradeMaxValueInPoints { get; set; }

        /// <summary>
        /// Gets or sets the minimal value for the record height.
        /// </summary>
        /// <value>
        /// The minimal value for the record height.
        /// </value>
        public decimal HeightMinValueInMeters { get; set; }

        /// <summary>
        /// Gets or sets the maximal value for the record height.
        /// </summary>
        /// <value>
        /// The maximal value for the record height.
        /// </value>
        public decimal HeightMaxValueInMeters { get; set; }

        /// <summary>
        /// Gets or sets the banned char for the record favourite symbol.
        /// </summary>
        /// <value>
        /// The banned char for the record favourite symbol.
        /// </value>
        public char FavouriteSymbolBannedChar { get; set; }
    }
}
