using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
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

        public int MinLength => MinLengthInSymbols;

        public int MaxLength => MaxLengthInSymbols;

        public int MinGrade => MinGradeInPoints;

        public int MaxGrade => MaxGradeInPoints;

        public decimal MinHeight => MinHeightInMeters;

        public decimal MaxHeight => MaxHeightInMeters;

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short grade, decimal height, char favouriteSymbol)
        {
            ValidateInput(firstName, lastName, dateOfBirth, grade, height, favouriteSymbol);

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

        public List<FileCabinetRecord> GetRecords()
        {
            return new List<FileCabinetRecord>(this.list);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> listOfFirstNames;
            if (!this.firstNameDictionary.TryGetValue(firstName?.ToLower(null), out listOfFirstNames))
            {
                listOfFirstNames = new List<FileCabinetRecord>();
            }

            return listOfFirstNames.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> listOfLastNames;
            if (!this.lastNameDictionary.TryGetValue(lastName?.ToLower(null), out listOfLastNames))
            {
                listOfLastNames = new List<FileCabinetRecord>();
            }

            return listOfLastNames.ToArray();
        }

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

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short grade, decimal height, char favouriteSymbol)
        {
            var indexToEdit = this.list.FindIndex((record) => record.Id == id);
            if (indexToEdit == -1)
            {
                throw new ArgumentException("No record with such id found.", nameof(id));
            }

            ValidateInput(firstName, lastName, dateOfBirth, grade, height, favouriteSymbol);

            var recordToEdit = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Grade = grade,
                Height = height,
                FavouriteSymbol = favouriteSymbol,
            };

            List<FileCabinetRecord> listOfFirstNames;
            if (this.firstNameDictionary.TryGetValue(this.list[indexToEdit].FirstName.ToLower(null), out listOfFirstNames))
            {
                var indexToEditFirstNamesList = listOfFirstNames.FindIndex((record) => record.Id == id);
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
                var indexToEditLastNamesList = listOfLastNames.FindIndex((record) => record.Id == id);
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
                var indexToEditDateOfBirthList = listOfDateOfBirth.FindIndex((record) => record.Id == id);
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

        public int GetStat()
        {
            return this.list.Count;
        }

        private static void ValidateInput(string firstName, string lastName, DateTime dateOfBirth, short grade, decimal height, char favouriteSymbol)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null.");
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException("Firstname cannot contain only spaces.", nameof(firstName));
            }

            if (firstName.Length < MinLengthInSymbols || firstName.Length > MaxLengthInSymbols)
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

            if (lastName.Length < MinLengthInSymbols || lastName.Length > MaxLengthInSymbols)
            {
                throw new ArgumentException("Firstname length should be in range [2;60].", nameof(lastName));
            }

            DateTime minimalDate = new DateTime(1950, 1, 1);

            if (dateOfBirth < minimalDate || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date should start from 01-Jan-1950 till Now.", nameof(dateOfBirth));
            }

            if (grade < MinGradeInPoints || grade > MaxGradeInPoints)
            {
                throw new ArgumentException("Grade should be in range [-10;10].", nameof(grade));
            }

            if (height < MinHeightInMeters || height > MaxHeightInMeters)
            {
                throw new ArgumentException("Height can't be lower 30cm and higher than 3m.", nameof(height));
            }

            if (favouriteSymbol == ' ')
            {
                throw new ArgumentException("Space-symbol is not valid.", nameof(favouriteSymbol));
            }
        }
    }
}
