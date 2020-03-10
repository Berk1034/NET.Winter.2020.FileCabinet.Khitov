using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The DefaultValidator class.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new IRecordValidator[]
            {
            new FirstNameValidator(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols),
            new LastNameValidator(ValidationRules.DefaultMinLengthInSymbols, ValidationRules.DefaultMaxLengthInSymbols),
            new DateOfBirthValidator(ValidationRules.DefaultMinimalDate, ValidationRules.DefaultMaximalDate),
            new GradeValidator(ValidationRules.DefaultMinGradeInPoints, ValidationRules.DefaultMaxGradeInPoints),
            new HeightValidator(ValidationRules.DefaultMinHeightInMeters, ValidationRules.DefaultMaxHeightInMeters),
            new FavouriteSymbolValidator(ValidationRules.DefaultBannedChar),
            })
        {
        }
    }
}
