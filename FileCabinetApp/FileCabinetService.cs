using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetService class.
    /// </summary>
    public class FileCabinetService
    {
        private const int MinLengthInSymbols = 2;
        private const int MaxLengthInSymbols = 60;
        private const int MinGradeInPoints = -10;
        private const int MaxGradeInPoints = 10;
        private const decimal MinHeightInMeters = 0.3m;
        private const decimal MaxHeightInMeters = 3m;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Gets the value of MinLengthInSymbols const.
        /// </summary>
        /// <value>
        /// The value of MinLengthInSymbols const.
        /// </value>
        public int MinLength => MinLengthInSymbols;

        /// <summary>
        /// Gets the value of MaxLengthInSymbols const.
        /// </summary>
        /// <value>
        /// The value of MaxLengthInSymbols const.
        /// </value>
        public int MaxLength => MaxLengthInSymbols;

        /// <summary>
        /// Gets the value of MinGradeInPoints const.
        /// </summary>
        /// <value>
        /// The value of MinGradeInPoints const.
        /// </value>
        public int MinGrade => MinGradeInPoints;

        /// <summary>
        /// Gets the value of MaxGradeInPoints const.
        /// </summary>
        /// <value>
        /// The value of MaxGradeInPoints const.
        /// </value>
        public int MaxGrade => MaxGradeInPoints;

        /// <summary>
        /// Gets the value of MinHeight const.
        /// </summary>
        /// <value>
        /// The value of MinHeight const.
        /// </value>
        public decimal MinHeight => MinHeightInMeters;

        /// <summary>
        /// Gets the value of MaxHeight const.
        /// </summary>
        /// <value>
        /// The value of MaxHeight const.
        /// </value>
        public decimal MaxHeight => MaxHeightInMeters;

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <exception cref="ArgumentException()">Thrown when record information do not meet the requirements: firstname and lastname length should be in range [2;60], contain not only space symbols, dateofbirth should be in range [01.01.1950;DateTime.Now], grade should be in range [-10;10], height should be in range [0,3m;3m], favouritesymbol can't be a space symbol.</exception>
        /// <exception cref="ArgumentNullException()">Thrown when firstname or lastname is null.</exception>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordInfo recordInfo)
        {
            ValidateInput(recordInfo);

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

            ValidateInput(recordInfo);

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

        private static void ValidateInput(FileCabinetRecordInfo recordInfo)
        {
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "Record information is null.");
            }

            if (recordInfo.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.FirstName), "Firstname can't be null.");
            }

            if (recordInfo.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo.FirstName));
            }

            if (recordInfo.FirstName.Length < MinLengthInSymbols || recordInfo.FirstName.Length > MaxLengthInSymbols)
            {
                throw new ArgumentException("Firstname length should be in range [2;60].", nameof(recordInfo.FirstName));
            }

            if (recordInfo.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordInfo.LastName), "Lastname can't be null.");
            }

            if (recordInfo.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(recordInfo.LastName));
            }

            if (recordInfo.LastName.Length < MinLengthInSymbols || recordInfo.LastName.Length > MaxLengthInSymbols)
            {
                throw new ArgumentException("Firstname length should be in range [2;60].", nameof(recordInfo.LastName));
            }

            DateTime minimalDate = new DateTime(1950, 1, 1);

            if (recordInfo.DateOfBirth < minimalDate || recordInfo.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date should start from 01-Jan-1950 till Now.", nameof(recordInfo.DateOfBirth));
            }

            if (recordInfo.Grade < MinGradeInPoints || recordInfo.Grade > MaxGradeInPoints)
            {
                throw new ArgumentException("Grade should be in range [-10;10].", nameof(recordInfo.Grade));
            }

            if (recordInfo.Height < MinHeightInMeters || recordInfo.Height > MaxHeightInMeters)
            {
                throw new ArgumentException("Height can't be lower 30cm and higher than 3m.", nameof(recordInfo.Height));
            }

            if (recordInfo.FavouriteSymbol == ' ')
            {
                throw new ArgumentException("Space-symbol is not valid.", nameof(recordInfo.FavouriteSymbol));
            }
        }
    }
}
