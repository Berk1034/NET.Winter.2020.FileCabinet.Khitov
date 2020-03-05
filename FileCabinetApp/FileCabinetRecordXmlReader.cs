using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecordXmlReader class.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all the records from reader.
        /// </summary>
        /// <returns>The IList of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<FileCabinetRecord>), new XmlRootAttribute("records"));
            List<FileCabinetRecord> recordsDeserialized = (List<FileCabinetRecord>)xmlSerializer.Deserialize(this.reader);
            return recordsDeserialized;
        }
    }
}
