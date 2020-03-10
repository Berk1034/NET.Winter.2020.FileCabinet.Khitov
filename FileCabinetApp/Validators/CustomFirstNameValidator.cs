using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomFirstNameValidator : IRecordValidator
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

            if (recordInfo.Name.FirstName.Length < ValidationRules.CustomMinLengthInSymbols || recordInfo.Name.FirstName.Length > ValidationRules.CustomMaxLengthInSymbols)
            {
                throw new ArgumentException($"Firstname length should be in range [{ValidationRules.CustomMinLengthInSymbols};{ValidationRules.CustomMaxLengthInSymbols}].", nameof(recordInfo.Name.FirstName));
            }
        }
    }
}
