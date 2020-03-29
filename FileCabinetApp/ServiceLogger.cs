using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The ServiceLogger class.
    /// </summary>
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private ServiceMeter serviceMeter;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream to write logs.</param>
        /// <param name="serviceMeter">The service meter to measure time.</param>
        public ServiceLogger(FileStream fileStream, ServiceMeter serviceMeter)
        {
            this.fileStream = fileStream;
            this.serviceMeter = serviceMeter;
        }

        /// <summary>
        /// Gets the service meter validator.
        /// </summary>
        /// <value>
        /// The service meter validator.
        /// </value>
        public IRecordValidator Validator => this.serviceMeter.Validator;

        /// <summary>
        /// Writes the call of CreateRecord method to the log file and Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecord recordInfo)
        {
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "RecordInfo is null.");
            }

            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling Create() with FirstName = '{recordInfo.Name.FirstName}', LastName = '{recordInfo.Name.LastName}', DateOfBirth = '{recordInfo.DateOfBirth.ToString("MM'/'dd'/'yyyy", null)}', Grade = '{recordInfo.Grade}', Height = '{recordInfo.Height}', FavouriteSymbol = '{recordInfo.FavouriteSymbol}'");

                var result = this.serviceMeter.CreateRecord(recordInfo);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Create() returned '{result}'");
                return result;
            }
        }

        /// <summary>
        /// Writes the call of EditRecord method to the log file and Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        public void EditRecord(FileCabinetRecord recordInfo)
        {
            if (recordInfo is null)
            {
                throw new ArgumentNullException(nameof(recordInfo), "RecordInfo is null.");
            }

            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling Edit() with Id = '{recordInfo.Id}', FirstName = '{recordInfo.Name.FirstName}', LastName = '{recordInfo.Name.LastName}', DateOfBirth = '{recordInfo.DateOfBirth.ToString("MM'/'dd'/'yyyy", null)}', Grade = '{recordInfo.Grade}', Height = '{recordInfo.Height}', FavouriteSymbol = '{recordInfo.FavouriteSymbol}'");

                this.serviceMeter.EditRecord(recordInfo);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Edit() finished.");
            }
        }

        /// <summary>
        /// Writes the call of FindByDateOfBirth method to the log file and finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling FindByDateOfBirth() with dateOfBirth = '{dateOfBirth}'");

                var result = this.serviceMeter.FindByDateOfBirth(dateOfBirth);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - FindByDateOfBirth() returned '{result}'.");

                return result;
            }
        }

        /// <summary>
        /// Writes the call of FindByFirstName method to the log file and Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling FindByFirstName() with firstName = '{firstName}'");

                var result = this.serviceMeter.FindByFirstName(firstName);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - FindByFirstName() returned {result}.");

                return result;
            }
        }

        /// <summary>
        /// Writes the call of FindByLastName method to the log file and Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling FindByLastName() with lastName = '{lastName}'");

                var result = this.serviceMeter.FindByLastName(lastName);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - FindByLastName() returned '{result}'.");

                return result;
            }
        }

        /// <summary>
        /// Writes the call of GetRecords method to the log file and Gets all the records.
        /// </summary>
        /// <returns>The ReadOnlyCollection of all current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling GetRecords()");

                var result = this.serviceMeter.GetRecords();
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - GetRecords() totally '{result.Count}' records.");
                return result;
            }
        }

        /// <summary>
        /// Writes the call of GetStat method to the log file and Gets amount of total and deleted records.
        /// </summary>
        /// <returns>The total number of records and deleted number of records.</returns>
        public (int total, int deleted) GetStat()
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling GetStat()");

                var result = this.serviceMeter.GetStat();
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - GetStat() returned total '{result.total}' records, deleted '{result.deleted}' records.");
                return result;
            }
        }

        /// <summary>
        /// Writes the call of MakeSnapshot method to the log file and Makes the snapshot of the current state of records.
        /// </summary>
        /// <returns>The snapshot of the current state of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling MakeSnapshot()");

                var result = this.serviceMeter.MakeSnapshot();
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - MakeSnapshot() returned '{result}'.");
                return result;
            }
        }

        /// <summary>
        /// Writes the call of Purge method to the log file and Defragments the data file - removes the spaces in the data file.
        /// </summary>
        public void Purge()
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling Purge()");

                this.serviceMeter.Purge();
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Purge() finished.");
            }
        }

        /// <summary>
        /// Writes the call of Remove method to the log file and Removes the record by id.
        /// </summary>
        /// <param name="id">The id of record to remove.</param>
        public void Remove(int id)
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling Remove() with Id = '{id}'");

                this.serviceMeter.Remove(id);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Remove() finished.");
            }
        }

        /// <summary>
        /// Writes the call of Restore method to the log file and Restores the state of records from the snapshot.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">The snapshot of the current state of records.</param>
        /// <returns>The amount of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            using (var streamWriter = new StreamWriter(this.fileStream, Encoding.ASCII, -1, true))
            {
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                var date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Calling Restore() with FileCabinetServiceSnapshot = '{fileCabinetServiceSnapshot}'");

                var result = this.serviceMeter.Restore(fileCabinetServiceSnapshot);
                date = DateTime.Now;
                streamWriter.WriteLine(
                    $"{date.ToString("MM'/'dd'/'yyyy", null)} {date.Hour}:{date.Minute} - Restore() returned '{result}'.");
                return result;
            }
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
    }
}
