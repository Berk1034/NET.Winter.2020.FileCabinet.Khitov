﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetFilesystemService class.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int RecordSizeInBytes = 277;
        private const int ReservedSizeInBytes = 2;
        private const int IdSizeInBytes = 4;
        private const int StringSizeInBytes = 120;
        private const int YearSizeInBytes = 4;
        private const int MonthSizeInBytes = 4;
        private const int DaySizeInBytes = 4;

        /*
        private const int GradeSizeInBytes = 2;
        private const int HeightSizeInBytes = 16;
        private const int SymbolSizeInBytes = 1;
        */
        
        private FileStream fileStream;
        private IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream to use.</param>
        /// <param name="validator">The validator for record information.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <value>
        /// The validator.
        /// </value>
        public IRecordValidator Validator => this.validator;

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecord recordInfo)
        {
            this.validator.ValidateParameters(recordInfo);

            int recordId;
            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                int amountOfRecords = (int)writer.BaseStream.Length / RecordSizeInBytes;
                recordId = this.FindLastID() + 1;
                short reseved = 0;
                writer.Write(reseved);
                writer.Write(recordId);
                writer.Write(recordInfo.Name.FirstName);
                writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + StringSizeInBytes, SeekOrigin.Begin);
                writer.Write(recordInfo.Name.LastName);
                writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2), SeekOrigin.Begin);
                writer.Write(recordInfo.DateOfBirth.Year);
                writer.Write(recordInfo.DateOfBirth.Month);
                writer.Write(recordInfo.DateOfBirth.Day);
                writer.Write(recordInfo.Grade);
                writer.Write(recordInfo.Height);
                writer.Write(recordInfo.FavouriteSymbol);
            }

            return recordId;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        public void EditRecord(FileCabinetRecord recordInfo)
        {
            this.validator.ValidateParameters(recordInfo);

            int id = -1;
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                {
                    while (reader.PeekChar() > -1 && id != recordInfo.Id)
                    {
                        short deleted = reader.ReadInt16();
                        if (deleted == 0)
                        {
                            id = reader.ReadInt32();
                            if (id != recordInfo.Id)
                            {
                                reader.BaseStream.Seek(RecordSizeInBytes - (ReservedSizeInBytes + IdSizeInBytes), SeekOrigin.Current);
                            }
                        }
                        else
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                        }
                    }

                    if (id == recordInfo.Id)
                    {
                        writer.Write(recordInfo.Name.FirstName);
                        writer.Seek(StringSizeInBytes - recordInfo.Name.FirstName.Length - 1, SeekOrigin.Current);
                        writer.Write(recordInfo.Name.LastName);
                        writer.Seek(StringSizeInBytes - recordInfo.Name.LastName.Length - 1, SeekOrigin.Current);
                        writer.Write(recordInfo.DateOfBirth.Year);
                        writer.Write(recordInfo.DateOfBirth.Month);
                        writer.Write(recordInfo.DateOfBirth.Day);
                        writer.Write(recordInfo.Grade);
                        writer.Write(recordInfo.Height);
                        writer.Write(recordInfo.FavouriteSymbol);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "yyyy-MMM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthday);
            if (dateSuccess)
            {
                List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
                using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (reader.PeekChar() > -1)
                    {
                        short deleted = reader.ReadInt16();
                        if (deleted == 0)
                        {
                            reader.BaseStream.Seek(-ReservedSizeInBytes, SeekOrigin.Current);
                            reader.BaseStream.Seek(ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2), SeekOrigin.Current);
                            int year = reader.ReadInt32();
                            int month = reader.ReadInt32();
                            int day = reader.ReadInt32();
                            DateTime recordDate = new DateTime(year, month, day);
                            if (birthday == recordDate)
                            {
                                reader.BaseStream.Seek(-(IdSizeInBytes + (StringSizeInBytes * 2) + YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes), SeekOrigin.Current);
                                int id = reader.ReadInt32();
                                string firstName = reader.ReadString();
                                reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                                string lastName = reader.ReadString();
                                reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                                reader.BaseStream.Seek(YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes, SeekOrigin.Current);
                                short grade = reader.ReadInt16();
                                decimal height = reader.ReadDecimal();
                                char favouriteSymbol = reader.ReadChar();

                                var record = new FileCabinetRecord()
                                {
                                    Id = id,
                                    Name = new Name
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                    },
                                    DateOfBirth = new DateTime(year, month, day),
                                    Grade = grade,
                                    Height = height,
                                    FavouriteSymbol = favouriteSymbol,
                                };
                                listOfRecords.Add(record);
                            }
                            else
                            {
                                reader.BaseStream.Seek(RecordSizeInBytes - (ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2) + YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes), SeekOrigin.Current);
                            }
                        }
                        else
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                        }
                    }
                }

                return listOfRecords.AsReadOnly();
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            }
        }

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    short deleted = reader.ReadInt16();
                    if (deleted == 0)
                    {
                        reader.BaseStream.Seek(-ReservedSizeInBytes, SeekOrigin.Current);
                        reader.BaseStream.Seek(ReservedSizeInBytes + IdSizeInBytes, SeekOrigin.Current);
                        string recordFirstName = reader.ReadString();
                        if (firstName == recordFirstName)
                        {
                            reader.BaseStream.Seek(-(recordFirstName.Length + IdSizeInBytes + 1), SeekOrigin.Current);
                            int id = reader.ReadInt32();
                            reader.BaseStream.Seek(StringSizeInBytes, SeekOrigin.Current);
                            string lastName = reader.ReadString();
                            reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                            int year = reader.ReadInt32();
                            int month = reader.ReadInt32();
                            int day = reader.ReadInt32();
                            short grade = reader.ReadInt16();
                            decimal height = reader.ReadDecimal();
                            char favouriteSymbol = reader.ReadChar();

                            var record = new FileCabinetRecord()
                            {
                                Id = id,
                                Name = new Name
                                {
                                    FirstName = firstName,
                                    LastName = lastName,
                                },
                                DateOfBirth = new DateTime(year, month, day),
                                Grade = grade,
                                Height = height,
                                FavouriteSymbol = favouriteSymbol,
                            };
                            listOfRecords.Add(record);
                        }
                        else
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - recordFirstName.Length - ReservedSizeInBytes - IdSizeInBytes - 1, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                    }
                }
            }

            return listOfRecords.AsReadOnly();
        }

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    short deleted = reader.ReadInt16();
                    if (deleted == 0)
                    {
                        reader.BaseStream.Seek(-ReservedSizeInBytes, SeekOrigin.Current);
                        reader.BaseStream.Seek(ReservedSizeInBytes + IdSizeInBytes + StringSizeInBytes, SeekOrigin.Current);
                        string recordLastName = reader.ReadString();
                        if (lastName == recordLastName)
                        {
                            reader.BaseStream.Seek(-(StringSizeInBytes + recordLastName.Length + IdSizeInBytes + 1), SeekOrigin.Current);
                            int id = reader.ReadInt32();
                            string firstName = reader.ReadString();
                            reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                            reader.BaseStream.Seek(StringSizeInBytes, SeekOrigin.Current);
                            int year = reader.ReadInt32();
                            int month = reader.ReadInt32();
                            int day = reader.ReadInt32();
                            short grade = reader.ReadInt16();
                            decimal height = reader.ReadDecimal();
                            char favouriteSymbol = reader.ReadChar();

                            var record = new FileCabinetRecord()
                            {
                                Id = id,
                                Name = new Name
                                {
                                    FirstName = firstName,
                                    LastName = lastName,
                                },
                                DateOfBirth = new DateTime(year, month, day),
                                Grade = grade,
                                Height = height,
                                FavouriteSymbol = favouriteSymbol,
                            };
                            listOfRecords.Add(record);
                        }
                        else
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - recordLastName.Length - ReservedSizeInBytes - IdSizeInBytes - StringSizeInBytes - 1, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                    }
                }
            }

            return listOfRecords.AsReadOnly();
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The ReadOnlyCollection of all current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
                    int id = reader.ReadInt32();
                    string firstName = reader.ReadString();
                    reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                    string lastName = reader.ReadString();
                    reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                    int year = reader.ReadInt32();
                    int month = reader.ReadInt32();
                    int day = reader.ReadInt32();
                    short grade = reader.ReadInt16();
                    decimal height = reader.ReadDecimal();
                    char favouriteSymbol = reader.ReadChar();

                    var record = new FileCabinetRecord()
                    {
                        Id = id,
                        Name = new Name
                        {
                            FirstName = firstName,
                            LastName = lastName,
                        },
                        DateOfBirth = new DateTime(year, month, day),
                        Grade = grade,
                        Height = height,
                        FavouriteSymbol = favouriteSymbol,
                    };
                    listOfRecords.Add(record);
                }
            }

            return listOfRecords.AsReadOnly();
        }

        /// <summary>
        /// Gets amount of records.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public (int total, int deleted) GetStat()
        {
            int totalRecordsAmount = 0;
            int deletedAmount = 0;
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    short deleted = reader.ReadInt16();
                    if (deleted != 0)
                    {
                        deletedAmount++;
                    }

                    reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                }

                totalRecordsAmount = (int)reader.BaseStream.Length / RecordSizeInBytes;
            }

            return (total: totalRecordsAmount, deleted: deletedAmount);
        }

        /// <summary>
        /// Defragments the data file - removes the spaces in the data file.
        /// </summary>
        public void Purge()
        {
            string tempFile = Path.GetTempFileName();
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite), Encoding.ASCII))
                {
                    while (reader.PeekChar() > -1)
                    {
                        short deleted = reader.ReadInt16();
                        if (deleted == 0)
                        {
                            int id = reader.ReadInt32();
                            string firstName = reader.ReadString();
                            reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                            string lastName = reader.ReadString();
                            reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                            int year = reader.ReadInt32();
                            int month = reader.ReadInt32();
                            int day = reader.ReadInt32();
                            short grade = reader.ReadInt16();
                            decimal height = reader.ReadDecimal();
                            char favouriteSymbol = reader.ReadChar();

                            writer.BaseStream.Seek(0, SeekOrigin.End);
                            int amountOfRecords = (int)writer.BaseStream.Length / RecordSizeInBytes;
                            writer.Write(deleted);
                            writer.Write(id);
                            writer.Write(firstName);
                            writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + StringSizeInBytes, SeekOrigin.Begin);
                            writer.Write(lastName);
                            writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2), SeekOrigin.Begin);
                            writer.Write(year);
                            writer.Write(month);
                            writer.Write(day);
                            writer.Write(grade);
                            writer.Write(height);
                            writer.Write(favouriteSymbol);
                        }
                        else
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                        }
                    }
                }
            }

            this.fileStream.Close();

            File.Delete("cabinet-records.db");
            File.Move(tempFile, "cabinet-records.db");

            this.fileStream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        /// <summary>
        /// Make the snapshot of the current state of records.
        /// </summary>
        /// <returns>The snapshot of the current state of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the record by id.
        /// </summary>
        /// <param name="id">The id of record to remove.</param>
        public void Remove(int id)
        {
            int removeId = -1;
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                {
                    while (id != removeId)
                    {
                        reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
                        removeId = reader.ReadInt32();
                        if (id != removeId)
                        {
                            reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes - IdSizeInBytes, SeekOrigin.Current);
                        }
                    }

                    reader.BaseStream.Seek(-ReservedSizeInBytes - IdSizeInBytes, SeekOrigin.Current);
                    short deleted = 4;
                    writer.Write(deleted);
                }
            }
        }

        /// <summary>
        /// Restores the state of records from the snapshot.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">The snapshot of the current state of records.</param>
        /// <returns>The amount of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            int importedRecordsCount = 0;
            var importedRecords = new List<FileCabinetRecord>(fileCabinetServiceSnapshot.Records);
            foreach (var record in importedRecords)
            {
                try
                {
                    this.validator.ValidateParameters(record);
                    if (this.ContainsID(record.Id))
                    {
                        this.EditRecord(record);
                    }
                    else
                    {
                        using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                        {
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                            var position = writer.BaseStream.Position;
                            int amountOfRecords = (int)writer.BaseStream.Length / RecordSizeInBytes;
                            short reseved = 0;
                            writer.Write(reseved);
                            writer.Write(record.Id);
                            writer.Write(record.Name.FirstName);
                            writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + StringSizeInBytes, SeekOrigin.Begin);
                            writer.Write(record.Name.LastName);
                            writer.Seek((RecordSizeInBytes * amountOfRecords) + ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2), SeekOrigin.Begin);
                            writer.Write(record.DateOfBirth.Year);
                            writer.Write(record.DateOfBirth.Month);
                            writer.Write(record.DateOfBirth.Day);
                            writer.Write(record.Grade);
                            writer.Write(record.Height);
                            writer.Write(record.FavouriteSymbol);
                        }
                    }

                    importedRecordsCount++;
                }
                catch (ArgumentException argException)
                {
                    Console.WriteLine($"Validation failed at importing record with ID #{record.Id} with message: {argException.Message}");
                }
            }

            return importedRecordsCount;
        }

        /// <summary>
        /// Frees the resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees the resources.
        /// </summary>
        /// <param name="disposing">Flag to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.fileStream != null)
                {
                    this.fileStream.Close();
                }
            }
        }

        private bool ContainsID(int id)
        {
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
                    int recordID = reader.ReadInt32();
                    if (id == recordID)
                    {
                        return true;
                    }

                    reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes - IdSizeInBytes, SeekOrigin.Current);
                }

                return false;
            }
        }

        private int FindLastID()
        {
            int lastID = 0;
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    short deleted = reader.ReadInt16();
                    if (deleted == 0)
                    {
                        reader.BaseStream.Seek(-ReservedSizeInBytes, SeekOrigin.Current);
                        reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
                        int recordID = reader.ReadInt32();
                        if (lastID < recordID)
                        {
                            lastID = recordID;
                        }

                        reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes - IdSizeInBytes, SeekOrigin.Current);
                    }
                    else
                    {
                        reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                    }
                }
            }

            return lastID;
        }
    }
}
