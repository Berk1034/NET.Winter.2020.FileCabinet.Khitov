using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The CreateCommandHandler class.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="appCommandRequest">The app command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest), "AppCommandRequest is null.");
            }

            if (appCommandRequest.Command == "create")
            {
                int recordId;
                recordId = this.Service.CreateRecord(InputReader.CreateRecord());

                Memoizer.Clear();

                Console.WriteLine($"Record #{recordId} is created.");
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
