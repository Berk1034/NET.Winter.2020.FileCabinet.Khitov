﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short grade, decimal height, char favouriteSymbol)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null.");
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(firstName));
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Firstname length should be in range [2;60].", nameof(firstName));
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Lastname can't be null.");
            }

            if (lastName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(lastName));
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Firstname length should be in range [2;60].", nameof(lastName));
            }

            DateTime minimalDate = new DateTime(1950, 1, 1);

            if (dateOfBirth < minimalDate || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date should start from 01-Jan-1950 till Now.");
            }

            if (grade < -10 || grade > 10)
            {
                throw new ArgumentException("Grade should be in range [-10;10].");
            }

            if (height < 0.3m || height > 3m)
            {
                throw new ArgumentException("Height can't be lower 40cm and higher than 3m.");
            }

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
