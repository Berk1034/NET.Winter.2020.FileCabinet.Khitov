using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultLastNameValidator : IRecordValidator
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

            if (recordInfo.Name.LastName.Length < ValidationRules.DefaultMinLengthInSymbols || recordInfo.Name.LastName.Length > ValidationRules.DefaultMaxLengthInSymbols)
            {
                throw new ArgumentException($"Lastname length should be in range [{ValidationRules.DefaultMinLengthInSymbols};{ValidationRules.DefaultMaxLengthInSymbols}.", nameof(recordInfo.Name.LastName));
            }
        }
    }
}
