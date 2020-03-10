using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The DefaultRecordPrinter class.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Prints records in comma separated way.
        /// </summary>
        /// <param name="records">The records to be printed.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.Name.FirstName}, {record.Name.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}, {record.Grade}, {record.Height}, {record.FavouriteSymbol}");
            }
        }
    }
}
