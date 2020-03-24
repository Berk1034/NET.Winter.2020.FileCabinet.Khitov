using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The IPrinter interface.
    /// </summary>
    public interface IPrinter
    {
        /// <summary>
        /// Prints the records with specified fields.
        /// </summary>
        /// <param name="records">The records to be printed.</param>
        /// <param name="recordFields">The fields of records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records, string[] recordFields);
    }
}
