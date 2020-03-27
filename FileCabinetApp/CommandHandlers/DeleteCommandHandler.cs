using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The DeleteCommandHandler class.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService.</param>
        public DeleteCommandHandler(IFileCabinetService service)
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

            if (appCommandRequest.Command == "delete")
            {
                char[] separators = { '=', ' ' };
                string[] sourceInput = appCommandRequest.Parameters.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (sourceInput.Length == 3)
                {
                    if (sourceInput[0] == "where")
                    {
                        string recordField = sourceInput[1].ToLower(null);
                        string recordValue = sourceInput[2];
                        if (recordValue[0] == '\'')
                        {
                            recordValue = recordValue.Substring(1, recordValue.Length - 2);
                        }

                        try
                        {
                            List<FileCabinetRecord> records;
                            switch (recordField)
                            {
                                case "id":
                                    int removeId;
                                    bool parseSuccess = int.TryParse(recordValue, out removeId);
                                    if (!parseSuccess)
                                    {
                                        Console.WriteLine("Can't extract id. You typed invalid symbols!");
                                    }
                                    else
                                    {
                                        this.Service.Remove(removeId);
                                        Console.WriteLine($"Record #{removeId} is deleted.");
                                    }

                                    break;
                                case "firstname":
                                    records = new List<FileCabinetRecord>(this.Service.FindByFirstName(recordValue));
                                    this.DeleteRecords(records, recordField);

                                    break;
                                case "lastname":
                                    records = new List<FileCabinetRecord>(this.Service.FindByLastName(recordValue));
                                    this.DeleteRecords(records, recordField);

                                    break;
                                case "dateofbirth":
                                    records = new List<FileCabinetRecord>(this.Service.FindByDateOfBirth(recordValue));
                                    this.DeleteRecords(records, recordField);

                                    break;
                            }

                            Memoizer.Clear();
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Can't delete record.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("This command starts with 'where'! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("This command admits four arguments! Try again.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }

        private void DeleteRecords(List<FileCabinetRecord> records, string recordField)
        {
            var removedIds = new List<int>();
            StringBuilder removedRecords = new StringBuilder();

            foreach (var record in records)
            {
                removedIds.Add(record.Id);
                this.Service.Remove(record.Id);
            }

            if (removedIds.Count != 0)
            {
                for (int i = 0; i < removedIds.Count; i++)
                {
                    if (i == (removedIds.Count - 1))
                    {
                        removedRecords.Append($"#{removedIds[i]}");
                    }
                    else
                    {
                        removedRecords.Append($"#{removedIds[i]}, ");
                    }
                }

                if (removedIds.Count == 1)
                {
                    Console.WriteLine($"Record {removedRecords} is deleted.");
                }
                else
                {
                    Console.WriteLine($"Records {removedRecords} are deleted.");
                }
            }
            else
            {
                Console.WriteLine($"No records with such {recordField}.");
            }
        }
    }
}
