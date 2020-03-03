using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    [XmlType("record")]
    public class FileCabinetRecord
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; }

        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [XmlElement("grade")]
        public short Grade { get; set; }

        [XmlElement("height")]
        public decimal Height { get; set; }

        [XmlIgnore]
        public char FavouriteSymbol { get; set; }

        [XmlElement("favouriteSymbol")]
        public string FavouriteSymbolString
        {
            get => FavouriteSymbol.ToString();
            set => FavouriteSymbol = value[0];
        }
    }
}
