using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The CommandHandlerBase class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public abstract void Handle(AppCommandRequest appCommandRequest);

        /// <summary>
        /// Sets the next command handler.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
        }
    }
}
