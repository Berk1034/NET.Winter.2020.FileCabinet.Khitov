using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomLastNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Name.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.Name.LastName), "Lastname can't be null.");
            }

            if (recordInfo.Name.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Lastname cannot contain only spaces.", nameof(recordInfo.Name.LastName));
            }

            if (recordInfo.Name.LastName.Length < ValidationRules.CustomMinLengthInSymbols || recordInfo.Name.LastName.Length > ValidationRules.CustomMaxLengthInSymbols)
            {
                throw new ArgumentException($"Lastname length should be in range [{ValidationRules.CustomMinLengthInSymbols};{ValidationRules.CustomMaxLengthInSymbols}.", nameof(recordInfo.Name.LastName));
            }
        }
    }
}
