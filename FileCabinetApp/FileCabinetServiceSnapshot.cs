using System;
using System.Collections.Generic;
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
    }
}
