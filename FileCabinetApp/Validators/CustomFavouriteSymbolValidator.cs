using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomFavouriteSymbolValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.FavouriteSymbol == ValidationRules.CustomBannedChar)
            {
                throw new ArgumentException($"{ValidationRules.CustomBannedChar} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
