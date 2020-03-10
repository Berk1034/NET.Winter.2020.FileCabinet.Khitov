using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The ValidationRules class.
    /// </summary>
    public static class ValidationRules
    {
        /// <summary>
        /// The minimal length for the record first and last names for default validation.
        /// </summary>
        public const int DefaultMinLengthInSymbols = 2;

        /// <summary>
        /// The maximal length for the record first and last names for default validation.
        /// </summary>
        public const int DefaultMaxLengthInSymbols = 60;

        /// <summary>
        /// The minimal value for the record grade for default validation.
        /// </summary>
        public const short DefaultMinGradeInPoints = -10;

        /// <summary>
        /// The maximal value for the record grade for default validation.
        /// </summary>
        public const short DefaultMaxGradeInPoints = 10;

        /// <summary>
        /// The minimal value for the record height for default validation.
        /// </summary>
        public const decimal DefaultMinHeightInMeters = 0.3m;

        /// <summary>
        /// The maximal value for the record height for default validation.
        /// </summary>
        public const decimal DefaultMaxHeightInMeters = 3m;

        /// <summary>
        /// The banned char for the record favourite symbol for default validation.
        /// </summary>
        public const char DefaultBannedChar = ' ';

        /// <summary>
        /// The minimal datetime for the record date of birth for default validation.
        /// </summary>
        public static readonly DateTime DefaultMinimalDate = new DateTime(1950, 1, 1);

        /// <summary>
        /// The maximal datetime for the record date of birth for default validation.
        /// </summary>
        public static readonly DateTime DefaultMaximalDate = DateTime.Now;

        /// <summary>
        /// The minimal length for the record first and last names for custom validation.
        /// </summary>
        public const int CustomMinLengthInSymbols = 2;

        /// <summary>
        /// The maximal length for the record first and last names for custom validation.
        /// </summary>
        public const int CustomMaxLengthInSymbols = 40;

        /// <summary>
        /// The minimal value for the record grade for custom validation.
        /// </summary>
        public const short CustomMinGradeInPoints = 0;

        /// <summary>
        /// The maximal value for the record grade for custom validation.
        /// </summary>
        public const short CustomMaxGradeInPoints = 5;

        /// <summary>
        /// The minimal value for the record height for custom validation.
        /// </summary>
        public const decimal CustomMinHeightInMeters = 0.4m;

        /// <summary>
        /// The maximal value for the record height for custom validation.
        /// </summary>
        public const decimal CustomMaxHeightInMeters = 3m;

        /// <summary>
        /// The banned char for the record favourite symbol for custom validation.
        /// </summary>
        public const char CustomBannedChar = ' ';

        /// <summary>
        /// The minimal datetime for the record date of birth for custom validation.
        /// </summary>
        public static readonly DateTime CustomMinimalDate = new DateTime(1960, 1, 1);

        /// <summary>
        /// The maximal datetime for the record date of birth for custom validation.
        /// </summary>
        public static readonly DateTime CustomMaximalDate = DateTime.Now;

        /// <summary>
        /// The flag to determine which rules to use.
        /// </summary>
        public static bool DefaultValidation = true;
    }
}
