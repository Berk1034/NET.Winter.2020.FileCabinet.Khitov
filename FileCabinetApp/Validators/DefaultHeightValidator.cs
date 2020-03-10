using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultHeightValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Height < ValidationRules.DefaultMinHeightInMeters || recordInfo.Height > ValidationRules.DefaultMaxHeightInMeters)
            {
                throw new ArgumentException($"Height can't be lower {ValidationRules.DefaultMinHeightInMeters} and higher than {ValidationRules.DefaultMaxHeightInMeters}.", nameof(recordInfo.Height));
            }
        }
    }
}
