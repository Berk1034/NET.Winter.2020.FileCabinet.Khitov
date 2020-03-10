using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultDateOfBirthValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.DateOfBirth < ValidationRules.DefaultMinimalDate || recordInfo.DateOfBirth > ValidationRules.DefaultMaximalDate)
            {
                throw new ArgumentException($"Date should start from {ValidationRules.DefaultMinimalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {ValidationRules.DefaultMaximalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo.DateOfBirth));
            }
        }
    }
}
