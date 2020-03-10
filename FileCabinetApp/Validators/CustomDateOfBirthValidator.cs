using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.DateOfBirth < ValidationRules.CustomMinimalDate || recordInfo.DateOfBirth > ValidationRules.CustomMaximalDate)
            {
                throw new ArgumentException($"Date should start from {ValidationRules.CustomMinimalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {ValidationRules.CustomMaximalDate.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo.DateOfBirth));
            }
        }
    }
}
