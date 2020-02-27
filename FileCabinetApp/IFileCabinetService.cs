using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The IFileCabinetService interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <value>
        /// The validator.
        /// </value>
        public IRecordValidator Validator { get; }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordInfo recordInfo);

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The ReadOnlyCollection of all current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        public void EditRecord(FileCabinetRecordInfo recordInfo);

        /// <summary>
        /// Gets amount of records.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public int GetStat();

        /// <summary>
        /// Makes the snapshot of current records.
        /// </summary>
        /// <returns>The snapshot of file cabinet service.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();
    }
}
