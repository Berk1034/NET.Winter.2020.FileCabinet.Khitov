using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The IRecordValidator interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="recordInfo">The record information for validation.</param>
        public void ValidateParameters(FileCabinetRecordInfo recordInfo);
    }
}
