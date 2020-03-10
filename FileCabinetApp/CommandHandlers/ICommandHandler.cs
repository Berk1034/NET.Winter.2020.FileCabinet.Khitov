using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ICommandHandler interface.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next command handler.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        public void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public void Handle(AppCommandRequest appCommandRequest);
    }
}
