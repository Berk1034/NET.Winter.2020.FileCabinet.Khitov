using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecordCsvReader class.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all the lines from reader.
        /// </summary>
        /// <returns>The IList of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> importRecords = new List<FileCabinetRecord>();
            var firstLine = this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                var data = this.reader.ReadLine();
                var values = data.Split(',');

                var record = new FileCabinetRecord()
                {
                    Id = Convert.ToInt32(values[0]),
                    Name = new Name
                    {
                        FirstName = values[1],
                        LastName = values[2],
                    },
                    DateOfBirth = DateTime.ParseExact(values[3], "MM/dd/yyyy", null),
                    Grade = Convert.ToInt16(values[4]),
                    Height = Convert.ToDecimal(values[5], CultureInfo.InvariantCulture),
                    FavouriteSymbol = Convert.ToChar(values[6]),
                };

                importRecords.Add(record);
            }

            return importRecords;
        }
    }
}
