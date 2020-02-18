using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetService class.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <exception cref="ArgumentException()">Thrown when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordInfo recordInfo)
        {
            this.CreateValidator().ValidateParameters(recordInfo);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = recordInfo.FirstName,
                LastName = recordInfo.LastName,
                DateOfBirth = recordInfo.DateOfBirth,
                Grade = recordInfo.Grade,
                Height = recordInfo.Height,
                FavouriteSymbol = recordInfo.FavouriteSymbol,
            };

            List<FileCabinetRecord> listOfFirstNames;
            if (this.firstNameDictionary.TryGetValue(record.FirstName?.ToLower(null), out listOfFirstNames))
            {
                listOfFirstNames.Add(record);
            }
            else
            {
                listOfFirstNames = new List<FileCabinetRecord>
                {
                    record,
                };
                this.firstNameDictionary.Add(record.FirstName.ToLower(null), listOfFirstNames);
            }

            List<FileCabinetRecord> listOfLastNames;
            if (this.lastNameDictionary.TryGetValue(record.LastName?.ToLower(null), out listOfLastNames))
            {
                listOfLastNames.Add(record);
            }
            else
            {
                listOfLastNames = new List<FileCabinetRecord>
                {
                    record,
                };
                this.lastNameDictionary.Add(record.LastName.ToLower(null), listOfLastNames);
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

            this.list.Add(record);

            return record.Id;
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The list of all current records.</returns>
        public List<FileCabinetRecord> GetRecords()
        {
            return new List<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The list of found records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> listOfFirstNames;
            if (!this.firstNameDictionary.TryGetValue(firstName?.ToLower(null), out listOfFirstNames))
            {
                listOfFirstNames = new List<FileCabinetRecord>();
            }

            return listOfFirstNames.ToArray();
        }

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The list of found records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> listOfLastNames;
            if (!this.lastNameDictionary.TryGetValue(lastName?.ToLower(null), out listOfLastNames))
            {
                listOfLastNames = new List<FileCabinetRecord>();
            }

            return listOfLastNames.ToArray();
        }

        /// <summary>
        /// Finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The list of found records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            DateTime birthday;
            bool dateSuccess = DateTime.TryParseExact(dateOfBirth, "yyyy-MMM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out birthday);
            if (dateSuccess)
            {
                List<FileCabinetRecord> listOfDateOfBirth;
                if (!this.dateOfBirthDictionary.TryGetValue(birthday, out listOfDateOfBirth))
                {
                    listOfDateOfBirth = new List<FileCabinetRecord>();
                }

                return listOfDateOfBirth.ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>\
        /// <exception cref="ArgumentException()">Thrown when no record with such id found or when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        public void EditRecord(FileCabinetRecordInfo recordInfo)
        {
            var indexToEdit = this.list.FindIndex((record) => record.Id == recordInfo.Id);
            if (indexToEdit == -1)
            {
                throw new ArgumentException("No record with such id found.", nameof(recordInfo.Id));
            }

            this.CreateValidator().ValidateParameters(recordInfo);

            var recordToEdit = new FileCabinetRecord
            {
                Id = recordInfo.Id,
                FirstName = recordInfo.FirstName,
                LastName = recordInfo.LastName,
                DateOfBirth = recordInfo.DateOfBirth,
                Grade = recordInfo.Grade,
                Height = recordInfo.Height,
                FavouriteSymbol = recordInfo.FavouriteSymbol,
            };

            List<FileCabinetRecord> listOfFirstNames;
            if (this.firstNameDictionary.TryGetValue(this.list[indexToEdit].FirstName.ToLower(null), out listOfFirstNames))
            {
                var indexToEditFirstNamesList = listOfFirstNames.FindIndex((record) => record.Id == recordInfo.Id);
                listOfFirstNames.RemoveAt(indexToEditFirstNamesList);

                List<FileCabinetRecord> newListOfFirstNames;
                if (this.firstNameDictionary.TryGetValue(recordToEdit.FirstName?.ToLower(null), out newListOfFirstNames))
                {
                    newListOfFirstNames.Add(recordToEdit);
                }
                else
                {
                    newListOfFirstNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                    this.firstNameDictionary.Add(recordToEdit.FirstName.ToLower(null), newListOfFirstNames);
                }
            }
            else
            {
                listOfFirstNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                this.firstNameDictionary.Add(recordToEdit.FirstName, listOfFirstNames);
            }

            List<FileCabinetRecord> listOfLastNames;
            if (this.lastNameDictionary.TryGetValue(this.list[indexToEdit].LastName.ToLower(null), out listOfLastNames))
            {
                var indexToEditLastNamesList = listOfLastNames.FindIndex((record) => record.Id == recordInfo.Id);
                listOfLastNames.RemoveAt(indexToEditLastNamesList);

                List<FileCabinetRecord> newListOfLastNames;
                if (this.lastNameDictionary.TryGetValue(recordToEdit.LastName?.ToLower(null), out newListOfLastNames))
                {
                    newListOfLastNames.Add(recordToEdit);
                }
                else
                {
                    newListOfLastNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                    this.lastNameDictionary.Add(recordToEdit.LastName.ToLower(null), newListOfLastNames);
                }
            }
            else
            {
                listOfLastNames = new List<FileCabinetRecord>
                {
                    recordToEdit,
                };
                this.lastNameDictionary.Add(recordToEdit.LastName, listOfLastNames);
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
        /// Gets amount of records.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Creates the validator for record information.
        /// </summary>
        /// <returns>The IRecordValidator implementation.</returns>
        protected abstract IRecordValidator CreateValidator();
    }
}
