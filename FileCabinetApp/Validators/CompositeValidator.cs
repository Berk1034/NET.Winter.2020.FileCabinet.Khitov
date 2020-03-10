using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The CompositeValidator class.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The list of validators to use.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators);
        }

        /// <summary>
        /// Validates the record information using validators.
        /// </summary>
        /// <param name="recordInfo">The record informaton.</param>
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordInfo);
            }
        }
    }
}
