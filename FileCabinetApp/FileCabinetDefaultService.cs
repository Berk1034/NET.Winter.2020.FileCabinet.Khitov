using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetDefaultService class.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetMemoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}
