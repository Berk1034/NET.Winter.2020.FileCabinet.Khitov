using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultFavouriteSymbolValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.FavouriteSymbol == ValidationRules.DefaultBannedChar)
            {
                throw new ArgumentException($"{ValidationRules.DefaultBannedChar} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
