using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultFirstNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Name.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.Name.FirstName), "Firstname can't be null.");
            }

            if (recordInfo.Name.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo.Name.FirstName));
            }

            if (recordInfo.Name.FirstName.Length < ValidationRules.DefaultMinLengthInSymbols || recordInfo.Name.FirstName.Length > ValidationRules.DefaultMaxLengthInSymbols)
            {
                throw new ArgumentException($"Firstname length should be in range [{ValidationRules.DefaultMinLengthInSymbols};{ValidationRules.DefaultMaxLengthInSymbols}].", nameof(recordInfo.Name.FirstName));
            }
        }
    }
}
