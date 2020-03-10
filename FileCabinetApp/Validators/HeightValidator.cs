using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class HeightValidator : IRecordValidator
    {
        private decimal minHeight;
        private decimal maxHeight;

        public HeightValidator(decimal minHeight, decimal maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Height < this.minHeight || recordInfo.Height > this.maxHeight)
            {
                throw new ArgumentException($"Height can't be lower {this.minHeight} and higher than {this.maxHeight}.", nameof(recordInfo.Height));
            }
        }
    }
}
