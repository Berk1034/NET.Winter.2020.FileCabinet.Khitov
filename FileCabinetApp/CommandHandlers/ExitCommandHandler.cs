using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The ExitCommandHandler class.
    /// </summary>
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        private Action<bool> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="action">The Action delegate to contol exit.</param>
        public ExitCommandHandler(IFileCabinetService service, Action<bool> action)
            : base(service)
        {
            this.action = action;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest.Command == "exit")
            {
                if (this.service is IDisposable)
                {
                    (this.service as IDisposable).Dispose();
                }

                Console.WriteLine("Exiting an application...");
                this.action(false);
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
