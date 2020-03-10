using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The IRecordPrinter interface.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints records in specific way.
        /// </summary>
        /// <param name="records">The records to be printed.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
