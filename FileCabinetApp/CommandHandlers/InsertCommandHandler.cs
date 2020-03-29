using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The InsertCommandHandler class.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.Service = service;
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

            if (appCommandRequest.Command == "insert")
            {
                try
                {
                    char[] separators = { '(', ')', ',', ' ' };
                    string[] sourceInput = appCommandRequest.Parameters.Split("values");

                    if (sourceInput.Length == 2)
                    {
                        string[] recordFields = sourceInput[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        string[] recordValues = sourceInput[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        if (recordFields.Length == recordValues.Length)
                        {
                            for (int i = 0; i < recordFields.Length; i++)
                            {
                                recordFields[i] = recordFields[i].ToLower(null);
                            }

                            for (int i = 0; i < recordValues.Length; i++)
                            {
                                if (recordValues[i][0] == '\'')
                                {
                                    recordValues[i] = recordValues[i].Substring(1, recordValues[i].Length - 2);
                                }
                            }

                            var recordId = this.Service.CreateRecord(InputReader.CreateRecord(recordFields, recordValues));

                            Memoizer.Clear();

                            Console.WriteLine($"Record #{recordId} is inserted.");
                        }
                        else
                        {
                            Console.WriteLine("Can't insert record.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't insert record.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }
    }
}
