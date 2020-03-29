using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecordXmlWriter class.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes record to xml file.
        /// </summary>
        /// <param name="record">The record to be written.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null.");
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.Name.FirstName);
            this.writer.WriteAttributeString("last", record.Name.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(record.DateOfBirth.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("grade");
            this.writer.WriteString(record.Grade.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("height");
            this.writer.WriteString(record.Height.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("favouriteSymbol");
            this.writer.WriteString(record.FavouriteSymbol.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();
        }
    }
}
