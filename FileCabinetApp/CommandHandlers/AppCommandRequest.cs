namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The AppCommandRequest class.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the command.
        /// </summary>
        /// <value>
        /// The parameters for the command.
        /// </value>
        public string Parameters { get; set; }
    }
}
