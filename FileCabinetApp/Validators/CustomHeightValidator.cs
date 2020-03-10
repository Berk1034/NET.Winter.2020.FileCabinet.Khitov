using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomHeightValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Height < ValidationRules.CustomMinHeightInMeters || recordInfo.Height > ValidationRules.CustomMaxHeightInMeters)
            {
                throw new ArgumentException($"Height can't be lower {ValidationRules.CustomMinHeightInMeters} and higher than {ValidationRules.CustomMaxHeightInMeters}.", nameof(recordInfo.Height));
            }
        }
    }
}
