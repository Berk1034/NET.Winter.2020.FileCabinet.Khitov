using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The ServiceMeter class.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;
        private Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">The service to measure its methods execution time.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets the service validator.
        /// </summary>
        /// <value>
        /// The service validator.
        /// </value>
        public IRecordValidator Validator => this.service.Validator;

        /// <summary>
        /// Measures the time of executing service CreateRecord method and Creates the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>The id of the created record.</returns>
        public int CreateRecord(FileCabinetRecord recordInfo)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.CreateRecord(recordInfo);
            this.stopWatch.Stop();
            Console.WriteLine($"CreateRecord method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service EditRecord method and Edits the record.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        public void EditRecord(FileCabinetRecord recordInfo)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.EditRecord(recordInfo);
            this.stopWatch.Stop();
            Console.WriteLine($"EditRecord method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Measures the time of executing service FindByDateOfBirth method and finds the records by the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IRecordIterator FindByDateOfBirth(string dateOfBirth)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopWatch.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service FindByFirstName method and Finds the records by the firstname.
        /// </summary>
        /// <param name="firstName">The first name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.FindByFirstName(firstName);
            this.stopWatch.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service FindByLastName method and Finds the records by the last name.
        /// </summary>
        /// <param name="lastName">The last name to find the records by it.</param>
        /// <returns>The ReadOnlyCollection of found records.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.FindByLastName(lastName);
            this.stopWatch.Stop();
            Console.WriteLine($"FindByLastName method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service GetRecords method and Gets all the records.
        /// </summary>
        /// <returns>The ReadOnlyCollection of all current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.GetRecords();
            this.stopWatch.Stop();
            Console.WriteLine($"GetRecords method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service GetStat method and Gets amount of total and deleted records.
        /// </summary>
        /// <returns>The total number of records and deleted number of records.</returns>
        public (int total, int deleted) GetStat()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.GetStat();
            this.stopWatch.Stop();
            Console.WriteLine($"GetStat method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service MakeSnapshot method and Makes the snapshot of the current state of records.
        /// </summary>
        /// <returns>The snapshot of the current state of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.MakeSnapshot();
            this.stopWatch.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Measures the time of executing service Purge method and Defragments the data file - removes the spaces in the data file.
        /// </summary>
        public void Purge()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.Purge();
            this.stopWatch.Stop();
            Console.WriteLine($"Purge method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Measures the time of executing service Remove method and Removes the record by id.
        /// </summary>
        /// <param name="id">The id of record to remove.</param>
        public void Remove(int id)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.Remove(id);
            this.stopWatch.Stop();
            Console.WriteLine($"Remove method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Measures the time of executing service Restore method and Restores the state of records from the snapshot.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">The snapshot of the current state of records.</param>
        /// <returns>The amount of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.Restore(fileCabinetServiceSnapshot);
            this.stopWatch.Stop();
            Console.WriteLine($"Restore method execution duration is {this.stopWatch.ElapsedTicks} ticks.");
            return result;
        }
    }
}
