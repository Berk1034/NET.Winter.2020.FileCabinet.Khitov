using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetService class.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The validator for record information.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
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
        /// <exception cref="ArgumentException()">Thrown when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecord recordInfo)
        {
            this.validator.ValidateParameters(recordInfo);

            int recordId;
            if (recordInfo.Id > 0)
            {
                recordId = recordInfo.Id;
            }
            else
            {
                if (this.list.Count == 0)
                {
                    recordId = 1;
                }
                else
                {
                    recordId = this.list.Max(record => record.Id) + 1;
                }
            }

            var record = new FileCabinetRecord
            {
                Id = recordId,
                Name = new Name
                {
                    FirstName = recordInfo.Name.FirstName,
                    LastName = recordInfo.Name.LastName,
                },
                DateOfBirth = recordInfo.DateOfBirth,
                Grade = recordInfo.Grade,
                Height = recordInfo.Height,
                FavouriteSymbol = recordInfo.FavouriteSymbol,
            };

            this.PopulateDictionariesWithRecord(record);
            this.list.Add(record);

            return record.Id;
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The ReadOnlyCollection of all current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> listOfFirstNames;
            if (!this.firstNameDictionary.TryGetValue(firstName?.ToLower(null), out listOfFirstNames))
            {
                listOfFirstNames = new List<FileCabinetRecord>();
            }

            foreach (var record in listOfFirstNames)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> listOfLastNames;
            if (!this.lastNameDictionary.TryGetValue(lastName?.ToLower(null), out listOfLastNames))
            {
                listOfLastNames = new List<FileCabinetRecord>();
            }

            foreach (var record in listOfLastNames)
            {
                yield return record;
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
            List<FileCabinetRecord> listOfDateOfBirth;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "yyyy-MMM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthday);
            if (dateSuccess)
            {
                if (!this.dateOfBirthDictionary.TryGetValue(birthday, out listOfDateOfBirth))
                {
                    listOfDateOfBirth = new List<FileCabinetRecord>();
                }
            }
            else
            {
                listOfDateOfBirth = new List<FileCabinetRecord>();
            }

            foreach (var record in listOfDateOfBirth)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <exception cref="ArgumentException()">Thrown when no record with such id found or when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        public void EditRecord(FileCabinetRecord recordInfo)
        {
            var indexToEdit = this.list.FindIndex((record) => record.Id == recordInfo.Id);
            if (indexToEdit == -1)
            {
                throw new ArgumentException("No record with such id found.", nameof(recordInfo.Id));
            }

            this.validator.ValidateParameters(recordInfo);

            var recordToEdit = new FileCabinetRecord
            {
                Id = recordInfo.Id,
                Name = new Name
                {
                    FirstName = recordInfo.Name.FirstName,
                    LastName = recordInfo.Name.LastName,
                },
                DateOfBirth = recordInfo.DateOfBirth,
                Grade = recordInfo.Grade,
                Height = recordInfo.Height,
                FavouriteSymbol = recordInfo.FavouriteSymbol,
            };

            List<FileCabinetRecord> listOfFirstNames;
            if (this.firstNameDictionary.TryGetValue(this.list[indexToEdit].Name.FirstName.ToLower(null), out listOfFirstNames))
            {
                var indexToEditFirstNamesList = listOfFirstNames.FindIndex((record) => record.Id == recordInfo.Id);
                listOfFirstNames.RemoveAt(indexToEditFirstNamesList);

                List<FileCabinetRecord> newListOfFirstNames;
                if (this.firstNameDictionary.TryGetValue(recordToEdit.Name.FirstName?.ToLower(null), out newListOfFirstNames))
                {
                    newListOfFirstNames.Add(recordToEdit);
                }
                else
                {
                    newListOfFirstNames = new List<FileCabinetRecord>
                    {
                        recordToEdit,
                    };
                    this.firstNameDictionary.Add(recordToEdit.Name.FirstName.ToLower(null), newListOfFirstNames);
                }
            }
            else
            {
                listOfFirstNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                this.firstNameDictionary.Add(recordToEdit.Name.FirstName, listOfFirstNames);
            }

            List<FileCabinetRecord> listOfLastNames;
            if (this.lastNameDictionary.TryGetValue(this.list[indexToEdit].Name.LastName.ToLower(null), out listOfLastNames))
            {
                var indexToEditLastNamesList = listOfLastNames.FindIndex((record) => record.Id == recordInfo.Id);
                listOfLastNames.RemoveAt(indexToEditLastNamesList);

                List<FileCabinetRecord> newListOfLastNames;
                if (this.lastNameDictionary.TryGetValue(recordToEdit.Name.LastName?.ToLower(null), out newListOfLastNames))
                {
                    newListOfLastNames.Add(recordToEdit);
                }
                else
                {
                    newListOfLastNames = new List<FileCabinetRecord>
                    {
                        recordToEdit,
                    };
                    this.lastNameDictionary.Add(recordToEdit.Name.LastName.ToLower(null), newListOfLastNames);
                }
            }
            else
            {
                listOfLastNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                this.lastNameDictionary.Add(recordToEdit.Name.LastName, listOfLastNames);
            }

            List<FileCabinetRecord> listOfDateOfBirth;
            if (this.dateOfBirthDictionary.TryGetValue(this.list[indexToEdit].DateOfBirth, out listOfDateOfBirth))
            {
                var indexToEditDateOfBirthList = listOfDateOfBirth.FindIndex((record) => record.Id == recordInfo.Id);
                listOfDateOfBirth.RemoveAt(indexToEditDateOfBirthList);

                List<FileCabinetRecord> newListOfDateOfBirth;
                if (this.dateOfBirthDictionary.TryGetValue(recordToEdit.DateOfBirth, out newListOfDateOfBirth))
                {
                    newListOfDateOfBirth.Add(recordToEdit);
                }
                else
                {
                    newListOfDateOfBirth = new List<FileCabinetRecord>
                    {
                        recordToEdit,
                    };
                    this.dateOfBirthDictionary.Add(recordToEdit.DateOfBirth, newListOfDateOfBirth);
                }
            }
            else
            {
                listOfDateOfBirth = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                this.dateOfBirthDictionary.Add(recordToEdit.DateOfBirth, listOfDateOfBirth);
            }

            this.list[indexToEdit] = recordToEdit;
        }

        /// <summary>
        /// Removes the record by id.
        /// </summary>
        /// <param name="id">The id of record to remove.</param>
        public void Remove(int id)
        {
            var indexToRemove = this.list.FindIndex((record) => record.Id == id);
            if (indexToRemove == -1)
            {
                throw new ArgumentException("No record with such id found.", nameof(id));
            }

            var recordToRemove = this.list[indexToRemove];

            this.firstNameDictionary[recordToRemove.Name.FirstName.ToLower(null)].Remove(recordToRemove);
            this.lastNameDictionary[recordToRemove.Name.LastName.ToLower(null)].Remove(recordToRemove);
            this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Remove(recordToRemove);

            this.list.RemoveAt(indexToRemove);
        }

        /// <summary>
        /// Gets amount of total and deleted records.
        /// </summary>
        /// <returns>The total number of records and deleted number of records.</returns>
        public (int total, int deleted) GetStat()
        {
            return (total: this.list.Count, deleted: 0);
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Purge()
        {
        }

        /// <summary>
        /// Make the snapshot of the current state of records.
        /// </summary>
        /// <returns>The snapshot of the current state of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
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
                    var updateRecord = this.list.Find(r => r.Id == record.Id);
                    if (updateRecord != null)
                    {
                        this.EditRecord(record);
                    }
                    else
                    {
                        this.list.Add(record);
                        this.PopulateDictionariesWithRecord(record);
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

        private void PopulateDictionariesWithRecord(FileCabinetRecord record)
        {
            List<FileCabinetRecord> listOfFirstNames;
            if (this.firstNameDictionary.TryGetValue(record.Name.FirstName?.ToLower(null), out listOfFirstNames))
            {
                listOfFirstNames.Add(record);
            }
            else
            {
                listOfFirstNames = new List<FileCabinetRecord>
                {
                    record,
                };
                this.firstNameDictionary.Add(record.Name.FirstName.ToLower(null), listOfFirstNames);
            }

            List<FileCabinetRecord> listOfLastNames;
            if (this.lastNameDictionary.TryGetValue(record.Name.LastName?.ToLower(null), out listOfLastNames))
            {
                listOfLastNames.Add(record);
            }
            else
            {
                listOfLastNames = new List<FileCabinetRecord>
                {
                    record,
                };
                this.lastNameDictionary.Add(record.Name.LastName.ToLower(null), listOfLastNames);
            }

            List<FileCabinetRecord> listOfDateOfBirth;
            if (this.dateOfBirthDictionary.TryGetValue(record.DateOfBirth, out listOfDateOfBirth))
            {
                listOfDateOfBirth.Add(record);
            }
            else
            {
                listOfDateOfBirth = new List<FileCabinetRecord>
                {
                    record,
                };
                this.dateOfBirthDictionary.Add(record.DateOfBirth, listOfDateOfBirth);
            }
        }
    }
}
