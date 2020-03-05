using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecord class.
    /// </summary>
    [XmlType("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the identificator of the record holder.
        /// </summary>
        /// <value>
        /// The identificator of the record.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name information of the record holder.
        /// </summary>
        /// <value>
        /// The name information of the record holder.
        /// </value>
        [XmlElement("name")]
        public Name Name { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the record holder.
        /// </summary>
        /// <value>
        /// The date of birth of the record.
        /// </value>
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the grade of the record holder.
        /// </summary>
        /// <value>
        /// The grade of the record.
        /// </value>
        [XmlElement("grade")]
        public short Grade { get; set; }

        /// <summary>
        /// Gets or sets the height of the record holder.
        /// </summary>
        /// <value>
        /// The height of the record.
        /// </value>
        [XmlElement("height")]
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the favourite symbol of the record holder.
        /// </summary>
        /// <value>
        /// The favourite symbol of the record.
        /// </value>
        [XmlIgnore]
        public char FavouriteSymbol { get; set; }

        /// <summary>
        /// Gets or sets the FavouriteSymbol.
        /// </summary>
        /// <value>
        /// The FavouriteSymbol.
        /// </value>
        [XmlElement("favouriteSymbol")]
        public string FavouriteSymbolString
        {
            get => this.FavouriteSymbol.ToString();
            set => this.FavouriteSymbol = value[0];
        }
    }
}
