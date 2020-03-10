using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The CustomValidator class.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
            {
            new FirstNameValidator(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols),
            new LastNameValidator(ValidationRules.CustomMinLengthInSymbols, ValidationRules.CustomMaxLengthInSymbols),
            new DateOfBirthValidator(ValidationRules.CustomMinimalDate, ValidationRules.CustomMaximalDate),
            new GradeValidator(ValidationRules.CustomMinGradeInPoints, ValidationRules.CustomMaxGradeInPoints),
            new HeightValidator(ValidationRules.CustomMinHeightInMeters, ValidationRules.CustomMaxHeightInMeters),
            new FavouriteSymbolValidator(ValidationRules.CustomBannedChar),
            })
        {
        }
    }
}
