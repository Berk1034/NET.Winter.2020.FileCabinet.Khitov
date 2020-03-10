using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The FavouriteSymbolValidator class.
    /// </summary>
    public class FavouriteSymbolValidator : IRecordValidator
    {
        private char bannedSymbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavouriteSymbolValidator"/> class.
        /// </summary>
        /// <param name="bannedSymbol">The banned symbol for validation.</param>
        public FavouriteSymbolValidator(char bannedSymbol)
        {
            this.bannedSymbol = bannedSymbol;
        }

        /// <summary>
        /// Validates the record information.
        /// </summary>
        /// <param name="recordInfo">The record informaton.</param>
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.FavouriteSymbol == this.bannedSymbol)
            {
                throw new ArgumentException($"{this.bannedSymbol} is not valid symbol.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
