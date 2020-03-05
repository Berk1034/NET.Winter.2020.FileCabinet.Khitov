using System;
using System.Collections.Generic;
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
            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString());
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.Name.FirstName);
            this.writer.WriteAttributeString("last", record.Name.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(record.DateOfBirth.ToString("MM'/'dd'/'yyyy", null));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("grade");
            this.writer.WriteString(record.Grade.ToString());
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("height");
            this.writer.WriteString(record.Height.ToString());
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("favouriteSymbol");
            this.writer.WriteString(record.FavouriteSymbol.ToString());
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();
        }
    }
}
