using System;
using System.Collections;
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

        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();
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
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (reader.PeekChar() > -1)
                {
                    var recordOffset = reader.BaseStream.Position;
                    short deleted = reader.ReadInt16();
                    if (deleted == 0)
                    {
                        reader.BaseStream.Seek(-ReservedSizeInBytes, SeekOrigin.Current);
                        reader.BaseStream.Seek(ReservedSizeInBytes + IdSizeInBytes, SeekOrigin.Current);
                        string firstName = reader.ReadString();
                        reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                        string lastName = reader.ReadString();
                        reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                        int year = reader.ReadInt32();
                        int month = reader.ReadInt32();
                        int day = reader.ReadInt32();
                        DateTime dateOfBirth = new DateTime(year, month, day);

                        this.PopulateDictionariesWithOffset(firstName, lastName, dateOfBirth, recordOffset);

                        reader.BaseStream.Seek(RecordSizeInBytes - (ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2) + YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes), SeekOrigin.Current);
                    }
                    else
                    {
                        reader.BaseStream.Seek(RecordSizeInBytes - ReservedSizeInBytes, SeekOrigin.Current);
                    }
                }
            }
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
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "FileCabinetRecord is null.");
            }

            this.validator.ValidateParameters(recordInfo);

            int recordId;
            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                var recordOffset = writer.BaseStream.Position;

                this.PopulateDictionariesWithOffset(recordInfo.Name.FirstName, recordInfo.Name.LastName, recordInfo.DateOfBirth, recordOffset);

                int amountOfRecords = (int)writer.BaseStream.Length / RecordSizeInBytes;

                if (recordInfo.Id > 0)
                {
                    recordId = recordInfo.Id;
                }
                else
                {
                    recordId = this.FindLastID() + 1;
                }

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
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "FileCabinetRecord is null.");
            }

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
                        var previousOffset = reader.BaseStream.Position;
                        reader.BaseStream.Seek(-(IdSizeInBytes + ReservedSizeInBytes), SeekOrigin.Current);
                        var recordOffset = reader.BaseStream.Position;
                        reader.BaseStream.Seek(IdSizeInBytes + ReservedSizeInBytes, SeekOrigin.Current);

                        string firstName = reader.ReadString();
                        reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                        string lastName = reader.ReadString();
                        reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                        int year = reader.ReadInt32();
                        int month = reader.ReadInt32();
                        int day = reader.ReadInt32();
                        DateTime dateOfBirth = new DateTime(year, month, day);
                        reader.BaseStream.Seek(-(YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes + (StringSizeInBytes * 2)), SeekOrigin.Current);

                        List<long> listOfFirstNameOffsets;
                        if (this.firstNameDictionary.TryGetValue(firstName.ToLower(null), out listOfFirstNameOffsets))
                        {
                            var indexToEditFirstNameOffsetsList = listOfFirstNameOffsets.FindIndex((offset) => offset == recordOffset);
                            listOfFirstNameOffsets.RemoveAt(indexToEditFirstNameOffsetsList);

                            List<long> newListOfFirstNameOffsets;
                            if (this.firstNameDictionary.TryGetValue(recordInfo.Name.FirstName.ToLower(null), out newListOfFirstNameOffsets))
                            {
                                newListOfFirstNameOffsets.Add(recordOffset);
                            }
                            else
                            {
                                newListOfFirstNameOffsets = new List<long>
                                {
                                    recordOffset,
                                };
                                this.firstNameDictionary.Add(recordInfo.Name.FirstName.ToLower(null), newListOfFirstNameOffsets);
                            }
                        }
                        else
                        {
                            listOfFirstNameOffsets = new List<long>
                            {
                                recordOffset,
                            };
                            this.firstNameDictionary.Add(recordInfo.Name.FirstName.ToLower(null), listOfFirstNameOffsets);
                        }

                        List<long> listOfLastNameOffsets;
                        if (this.lastNameDictionary.TryGetValue(lastName.ToLower(null), out listOfLastNameOffsets))
                        {
                            var indexToEditLastNameOffsetsList = listOfLastNameOffsets.FindIndex((offset) => offset == recordOffset);
                            listOfLastNameOffsets.RemoveAt(indexToEditLastNameOffsetsList);

                            List<long> newListOfLastNameOffsets;
                            if (this.lastNameDictionary.TryGetValue(recordInfo.Name.LastName.ToLower(null), out newListOfLastNameOffsets))
                            {
                                newListOfLastNameOffsets.Add(recordOffset);
                            }
                            else
                            {
                                newListOfLastNameOffsets = new List<long>
                                {
                                    recordOffset,
                                };
                                this.lastNameDictionary.Add(recordInfo.Name.LastName.ToLower(null), newListOfLastNameOffsets);
                            }
                        }
                        else
                        {
                            listOfLastNameOffsets = new List<long>
                            {
                                recordOffset,
                            };
                            this.lastNameDictionary.Add(recordInfo.Name.LastName.ToLower(null), listOfLastNameOffsets);
                        }

                        List<long> listOfDateOfBirthOffsets;
                        if (this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out listOfDateOfBirthOffsets))
                        {
                            var indexToEditDateOfBirthOffsetsList = listOfDateOfBirthOffsets.FindIndex((offset) => offset == recordOffset);
                            listOfDateOfBirthOffsets.RemoveAt(indexToEditDateOfBirthOffsetsList);

                            List<long> newListOfDateOfBirthOffsets;
                            if (this.dateOfBirthDictionary.TryGetValue(recordInfo.DateOfBirth, out newListOfDateOfBirthOffsets))
                            {
                                newListOfDateOfBirthOffsets.Add(recordOffset);
                            }
                            else
                            {
                                newListOfDateOfBirthOffsets = new List<long>
                                {
                                    recordOffset,
                                };
                                this.dateOfBirthDictionary.Add(recordInfo.DateOfBirth, newListOfDateOfBirthOffsets);
                            }
                        }
                        else
                        {
                            listOfDateOfBirthOffsets = new List<long>
                            {
                                recordOffset,
                            };
                            this.dateOfBirthDictionary.Add(recordInfo.DateOfBirth, listOfDateOfBirthOffsets);
                        }

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
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "yyyy-MMM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthday);
            if (dateSuccess)
            {
                using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    List<long> listOfDateOfBirthOffsets;
                    if (!this.dateOfBirthDictionary.TryGetValue(birthday, out listOfDateOfBirthOffsets))
                    {
                        listOfDateOfBirthOffsets = new List<long>();
                    }

                    listOfDateOfBirthOffsets.Sort();
                    int recordIndex = 0;
                    int recordsCount = listOfDateOfBirthOffsets.Count;

                    while (reader.PeekChar() > -1 && recordIndex < recordsCount)
                    {
                        long recordOffset = listOfDateOfBirthOffsets[recordIndex];
                        reader.BaseStream.Seek(recordOffset, SeekOrigin.Begin);
                        reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
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
                            DateOfBirth = birthday,
                            Grade = grade,
                            Height = height,
                            FavouriteSymbol = favouriteSymbol,
                        };

                        yield return record;
                        recordIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                List<long> listOfFirstNameOffsets;
                if (!this.firstNameDictionary.TryGetValue(firstName?.ToLower(null), out listOfFirstNameOffsets))
                {
                    listOfFirstNameOffsets = new List<long>();
                }

                listOfFirstNameOffsets.Sort();
                int recordIndex = 0;
                int recordsCount = listOfFirstNameOffsets.Count;

                while (reader.PeekChar() > -1 && recordIndex < recordsCount)
                {
                    long recordOffset = listOfFirstNameOffsets[recordIndex];
                    reader.BaseStream.Seek(recordOffset, SeekOrigin.Begin);
                    reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
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

                    yield return record;
                    recordIndex++;
                }
            }
        }

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                List<long> listOfLastNameOffsets;
                if (!this.lastNameDictionary.TryGetValue(lastName?.ToLower(null), out listOfLastNameOffsets))
                {
                    listOfLastNameOffsets = new List<long>();
                }

                listOfLastNameOffsets.Sort();
                int recordIndex = 0;
                int recordsCount = listOfLastNameOffsets.Count;

                while (reader.PeekChar() > -1 && recordIndex < recordsCount)
                {
                    long recordOffset = listOfLastNameOffsets[recordIndex];
                    reader.BaseStream.Seek(recordOffset, SeekOrigin.Begin);
                    reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
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

                    yield return record;
                    recordIndex++;
                }
            }
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
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();

            string tempFile = Path.GetTempFileName();
            using (var reader = new BinaryReader(this.fileStream, Encoding.ASCII, true))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite), Encoding.ASCII))
                {
                    while (reader.PeekChar() > -1)
                    {
                        var recordOffset = writer.BaseStream.Position;
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

                            this.PopulateDictionariesWithOffset(firstName, lastName, new DateTime(year, month, day), recordOffset);

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

                    long recordOffset = reader.BaseStream.Seek(-ReservedSizeInBytes - IdSizeInBytes, SeekOrigin.Current);
                    reader.BaseStream.Seek(ReservedSizeInBytes + IdSizeInBytes, SeekOrigin.Current);
                    string firstName = reader.ReadString();
                    reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
                    string lastName = reader.ReadString();
                    reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
                    int year = reader.ReadInt32();
                    int month = reader.ReadInt32();
                    int day = reader.ReadInt32();
                    DateTime dateOfBirth = new DateTime(year, month, day);

                    this.firstNameDictionary[firstName.ToLower(null)].Remove(recordOffset);
                    this.lastNameDictionary[lastName.ToLower(null)].Remove(recordOffset);
                    this.dateOfBirthDictionary[dateOfBirth].Remove(recordOffset);

                    reader.BaseStream.Seek(-(ReservedSizeInBytes + IdSizeInBytes + (StringSizeInBytes * 2) + YearSizeInBytes + MonthSizeInBytes + DaySizeInBytes), SeekOrigin.Current);
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
            if (fileCabinetServiceSnapshot is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetServiceSnapshot), "FileCabinetServiceSnapshot is null.");
            }

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

                            var recordOffset = writer.BaseStream.Position;
                            this.PopulateDictionariesWithOffset(record.Name.FirstName, record.Name.LastName, record.DateOfBirth, recordOffset);

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

        private void PopulateDictionariesWithOffset(string firstName, string lastName, DateTime dateOfBirth, long recordOffset)
        {
            List<long> listOfFirstNameOffsets;
            if (this.firstNameDictionary.TryGetValue(firstName.ToLower(null), out listOfFirstNameOffsets))
            {
                listOfFirstNameOffsets.Add(recordOffset);
            }
            else
            {
                listOfFirstNameOffsets = new List<long>
                {
                    recordOffset,
                };
                this.firstNameDictionary.Add(firstName.ToLower(null), listOfFirstNameOffsets);
            }

            List<long> listOfLastNameOffsets;
            if (this.lastNameDictionary.TryGetValue(lastName.ToLower(null), out listOfLastNameOffsets))
            {
                listOfLastNameOffsets.Add(recordOffset);
            }
            else
            {
                listOfLastNameOffsets = new List<long>
                {
                    recordOffset,
                };
                this.lastNameDictionary.Add(lastName.ToLower(null), listOfLastNameOffsets);
            }

            List<long> listOfDateOfBirthOffsets;
            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out listOfDateOfBirthOffsets))
            {
                listOfDateOfBirthOffsets.Add(recordOffset);
            }
            else
            {
                listOfDateOfBirthOffsets = new List<long>
                {
                    recordOffset,
                };
                this.dateOfBirthDictionary.Add(dateOfBirth, listOfDateOfBirthOffsets);
            }
        }
    }
}
