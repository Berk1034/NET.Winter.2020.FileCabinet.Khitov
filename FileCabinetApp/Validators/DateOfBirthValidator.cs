using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.DateOfBirth < this.from || recordInfo.DateOfBirth > this.to)
            {
                throw new ArgumentException($"Date should start from {this.from.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))} till {this.to.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}.", nameof(recordInfo.DateOfBirth));
            }
        }
    }
}
