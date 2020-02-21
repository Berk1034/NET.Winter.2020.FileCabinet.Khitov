﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecordCsvWriter class.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
            writer.WriteLine("Id,First Name,Last Name,Date of birth,Grade,Height,Favourite symbol");
        }

        /// <summary>
        /// Writes record to csv file.
        /// </summary>
        /// <param name="record">The record to be written.</param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteLine("{0},{1},{2},{3},{4},{5},{6}", record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("MM'/'dd'/'yyyy", null), record.Grade, record.Height, record.FavouriteSymbol);
        }
    }
}