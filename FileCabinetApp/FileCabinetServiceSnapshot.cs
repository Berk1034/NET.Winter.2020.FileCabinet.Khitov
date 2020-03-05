using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetServiceSnapshot class.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The array of records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
            this.records = Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get
            {
                return Array.AsReadOnly(this.records);
            }
        }

        /// <summary>
        /// Saves data to csv file.
        /// </summary>
        /// <param name="writer">The stream for file-writing.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);

            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Loads data from csv file.
        /// </summary>
        /// <param name="reader">The stream for file-reading.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            var csvReader = new FileCabinetRecordCsvReader(reader);
            var importedRecords = csvReader.ReadAll();
            var indexToAddRecords = this.records.Length;
            Array.Resize(ref this.records, this.records.Length + importedRecords.Count);
            importedRecords.CopyTo(this.records, indexToAddRecords);
        }

        /// <summary>
        /// Saves data to xml file.
        /// </summary>
        /// <param name="writer">The stream for file-writing.</param>
        public void SaveToXml(StreamWriter writer)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
            };

            var xmlWriter = XmlWriter.Create(writer as TextWriter, xmlWriterSettings);
            var xmlRecordWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            foreach (var record in this.records)
            {
                xmlRecordWriter.Write(record);
            }

            xmlWriter.WriteEndElement();

            xmlWriter.Close();
        }

        /// <summary>
        /// Loads data from xml file.
        /// </summary>
        /// <param name="reader">The stream for file-reading.</param>
        public void LoadFromXml(StreamReader reader)
        {
            var xmlReader = XmlReader.Create(reader as TextReader);
            var xmlRecordReader = new FileCabinetRecordXmlReader(xmlReader);

            var importedRecords = xmlRecordReader.ReadAll();
            var indexToAddRecords = this.records.Length;
            Array.Resize(ref this.records, this.records.Length + importedRecords.Count);
            importedRecords.CopyTo(this.records, indexToAddRecords);
        }
    }
}
