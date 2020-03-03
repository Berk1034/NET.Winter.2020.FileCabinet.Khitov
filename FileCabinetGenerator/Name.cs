using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    public class Name
    {
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
