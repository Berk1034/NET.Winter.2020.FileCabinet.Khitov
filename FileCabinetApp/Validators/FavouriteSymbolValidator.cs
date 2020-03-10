using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class FavouriteSymbolValidator : IRecordValidator
    {
        private char bannedSymbol;

        public FavouriteSymbolValidator(char bannedSymbol)
        {
            this.bannedSymbol = bannedSymbol;
        }

        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.FavouriteSymbol == this.bannedSymbol)
            {
                throw new ArgumentException($"{this.bannedSymbol} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
