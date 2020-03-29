namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The IRecordValidator interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="recordInfo">The record information for validation.</param>
        public void ValidateParameters(FileCabinetRecord recordInfo);
    }
}
