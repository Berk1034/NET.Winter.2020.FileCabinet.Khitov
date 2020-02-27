using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetFilesystemService class.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 277;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream to use.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordInfo recordInfo)
        {
            int recordId;
            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                var position = writer.BaseStream.Position;
                int amountOfRecords = (int)writer.BaseStream.Length / RecordSize;
                recordId = amountOfRecords + 1;
                short reseved = 0;
                writer.Write(reseved);
                writer.Write(recordId);
                writer.Write(recordInfo.FirstName);
                writer.Seek((RecordSize * amountOfRecords) + 126, SeekOrigin.Begin);
                writer.Write(recordInfo.LastName);
                writer.Seek((RecordSize * amountOfRecords) + 246, SeekOrigin.Begin);
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
        public void EditRecord(FileCabinetRecordInfo recordInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
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
                    reader.BaseStream.Seek(2, SeekOrigin.Current);
                    int id = reader.ReadInt32();
                    string firstName = reader.ReadString();
                    reader.BaseStream.Seek(120 - firstName.Length - 1, SeekOrigin.Current);
                    string lastName = reader.ReadString();
                    reader.BaseStream.Seek(120 - lastName.Length - 1, SeekOrigin.Current);
                    int year = reader.ReadInt32();
                    int month = reader.ReadInt32();
                    int day = reader.ReadInt32();
                    short grade = reader.ReadInt16();
                    decimal height = reader.ReadDecimal();
                    char favouriteSymbol = reader.ReadChar();

                    var record = new FileCabinetRecord()
                    {
                        Id = id,
                        FirstName = firstName,
                        LastName = lastName,
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
        public int GetStat()
        {
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                return (int)reader.BaseStream.Length / RecordSize;
            }
        }
    }
}
