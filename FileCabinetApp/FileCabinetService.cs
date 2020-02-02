using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short grade, decimal height, char favouriteSymbol)
        {
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Grade = grade,
                Height = height,
                FavouriteSymbol = favouriteSymbol,
            };

            this.list.Add(record);

            return record.Id;
        }

        public List<FileCabinetRecord> GetRecords()
        {
            return new List<FileCabinetRecord>(this.list);
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
