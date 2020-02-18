using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetRecordInfo class.
    /// </summary>
    public class FileCabinetRecordInfo
    {
        /// <summary>
        /// Gets or sets the identificator of the record holder.
        /// </summary>
        /// <value>
        /// The identificator of the record.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the record holder.
        /// </summary>
        /// <value>
        /// The first name of the record.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the record holder.
        /// </summary>
        /// <value>
        /// The last name of the record.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the record holder.
        /// </summary>
        /// <value>
        /// The date of birth of the record.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the grade of the record holder.
        /// </summary>
        /// <value>
        /// The grade of the record.
        /// </value>
        public short Grade { get; set; }

        /// <summary>
        /// Gets or sets the height of the record holder.
        /// </summary>
        /// <value>
        /// The height of the record.
        /// </value>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the favourite symbol of the record holder.
        /// </summary>
        /// <value>
        /// The favourite symbol of the record.
        /// </value>
        public char FavouriteSymbol { get; set; }
    }
}
