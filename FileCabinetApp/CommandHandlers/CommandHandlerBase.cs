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
        /// <summary>
        /// The Memoizer to store select results.
        /// </summary>
        protected static readonly Memoizer Memoizer = new Memoizer();

        private ICommandHandler nextHandler;

        /// <summary>
        /// Gets the nextHandler.
        /// </summary>
        /// <value>
        /// The nextHandler.
        /// </value>
        public ICommandHandler NextHandler
        {
            get
            {
                return this.nextHandler;
            }
        }

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
