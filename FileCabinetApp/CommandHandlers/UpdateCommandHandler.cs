using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The UpdateCommandHandler class.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
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

            if (appCommandRequest.Command == "update")
            {
                try
                {
                    char[] separators = { '=', ' ', ',' };
                    string[] sourceInput = appCommandRequest.Parameters.Split("where");

                    if (sourceInput.Length == 2)
                    {
                        string[] recordFieldsAndValues = sourceInput[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        string[] conditions = sourceInput[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        List<string> recordFields = new List<string>();
                        List<string> recordValues = new List<string>();

                        if (recordFieldsAndValues.Length < 3)
                        {
                            throw new ArgumentException("Not enough parameters after 'update' command.");
                        }

                        if (recordFieldsAndValues.Length > 2 && !recordFieldsAndValues.Contains("set"))
                        {
                            throw new ArgumentException("No separator 'set' after 'update' command!");
                        }

                        if (conditions.Length < 2)
                        {
                            throw new ArgumentException("Not enough parameters after 'where' condition.");
                        }

                        if (conditions.Length > 2 && !conditions.Contains("and") && !conditions.Contains("or"))
                        {
                            throw new ArgumentException("No separators 'and/or' after parameters!");
                        }

                        if (conditions.Contains("and") && conditions.Contains("or"))
                        {
                            throw new ArgumentException("Can't contain both 'and/or' separators.");
                        }

                        for (int i = 1; i < recordFieldsAndValues.Length; i += 2)
                        {
                            recordFieldsAndValues[i] = recordFieldsAndValues[i].ToLower(null);
                            recordFields.Add(recordFieldsAndValues[i]);
                        }

                        for (int i = 2; i < recordFieldsAndValues.Length; i += 2)
                        {
                            if (recordFieldsAndValues[i][0] == '\'')
                            {
                                recordFieldsAndValues[i] = recordFieldsAndValues[i].Substring(1, recordFieldsAndValues[i].Length - 2);
                            }

                            recordValues.Add(recordFieldsAndValues[i]);
                        }

                        for (int i = 0; i < conditions.Length; i += 3)
                        {
                            conditions[i] = conditions[i].ToLower(null);
                        }

                        for (int i = 1; i < conditions.Length; i += 3)
                        {
                            if (conditions[i][0] == '\'')
                            {
                                conditions[i] = conditions[i].Substring(1, conditions[i].Length - 2);
                            }
                        }

                        if (recordFields.Contains("id"))
                        {
                            throw new ArgumentException("Can't update id.");
                        }

                        List<FileCabinetRecord> recordsMatch = new List<FileCabinetRecord>(this.Service.GetRecords());

                        string condition = string.Empty;
                        if (conditions.Contains("and"))
                        {
                            condition = "and";
                        }

                        if (conditions.Contains("or"))
                        {
                            condition = "or";
                        }

                        if (string.IsNullOrEmpty(condition))
                        {
                            for (int i = 0; i < conditions.Length; i += 3)
                            {
                                switch (conditions[i])
                                {
                                    case "id":
                                        int id;
                                        bool parseSuccess = int.TryParse(conditions[i + 1], out id);
                                        if (parseSuccess)
                                        {
                                            recordsMatch = recordsMatch.FindAll(record => { return record.Id == id; });
                                        }

                                        break;
                                    case "firstname":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByFirstName(conditions[i + 1]));

                                        break;
                                    case "lastname":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByLastName(conditions[i + 1]));

                                        break;
                                    case "dateofbirth":
                                        recordsMatch = new List<FileCabinetRecord>(this.Service.FindByLastName(conditions[i + 1]));

                                        break;
                                }
                            }

                            Memoizer.Clear();
                        }
                        else
                        {
                            Func<FileCabinetRecord, bool> predicate1 = null;
                            Func<FileCabinetRecord, bool> predicate2 = null;

                            if (condition == "and")
                            {
                                this.AssignPredicates(out predicate1, out predicate2, conditions);
                                recordsMatch = recordsMatch.FindAll(record => { return predicate1(record) && predicate2(record); });
                            }
                            else if (condition == "or")
                            {
                                this.AssignPredicates(out predicate1, out predicate2, conditions);
                                recordsMatch = recordsMatch.FindAll(record => { return predicate1(record) || predicate2(record); });
                            }
                        }

                        var updatedIds = new List<int>();
                        StringBuilder updatedRecords = new StringBuilder();

                        for (int i = 0; i < recordsMatch.Count; i++)
                        {
                            var updatedRecord = InputReader.UpdateRecord(recordFields.ToArray(), recordValues.ToArray(), recordsMatch[i]);
                            updatedRecord.Id = recordsMatch[i].Id;

                            this.Service.EditRecord(updatedRecord);
                            updatedIds.Add(updatedRecord.Id);
                        }

                        if (updatedIds.Count != 0)
                        {
                            for (int i = 0; i < updatedIds.Count; i++)
                            {
                                if (i == (updatedIds.Count - 1))
                                {
                                    updatedRecords.Append($"#{updatedIds[i]}");
                                }
                                else
                                {
                                    updatedRecords.Append($"#{updatedIds[i]}, ");
                                }
                            }

                            if (updatedIds.Count == 1)
                            {
                                Console.WriteLine($"Record {updatedRecords.ToString()} is updated.");
                            }
                            else
                            {
                                Console.WriteLine($"Records {updatedRecords.ToString()} are updated.");
                            }

                            Memoizer.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"No records with such criteria found.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't update records.");
                }
            }
            else if (this.NextHandler != null)
            {
                this.NextHandler.Handle(appCommandRequest);
            }
        }

        private void AssignPredicates(out Func<FileCabinetRecord, bool> firstPredicate, out Func<FileCabinetRecord, bool> secondPredicate, string[] conditions)
        {
            Func<FileCabinetRecord, bool> predicate1 = null;
            Func<FileCabinetRecord, bool> predicate2 = null;

            for (int i = 0; i < conditions.Length; i += 3)
            {
                switch (conditions[i])
                {
                    case "id":
                        int id;
                        bool parseSuccess = int.TryParse(conditions[i + 1], out id);
                        if (parseSuccess)
                        {
                            if (predicate1 == null)
                            {
                                predicate1 = record => record.Id == id;
                            }
                            else
                            {
                                predicate2 = record => record.Id == id;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Can't extract id. You typed invalid symbols!");
                        }

                        continue;
                    case "firstname":
                        string firstname = conditions[i + 1];
                        if (predicate1 == null)
                        {
                            predicate1 = record => record.Name.FirstName == firstname;
                        }
                        else
                        {
                            predicate2 = record => record.Name.FirstName == firstname;
                        }

                        continue;
                    case "lastname":
                        string lastname = conditions[i + 1];
                        if (predicate1 == null)
                        {
                            predicate1 = record => record.Name.LastName == lastname;
                        }
                        else
                        {
                            predicate2 = record => record.Name.LastName == lastname;
                        }

                        continue;
                    case "dateofbirth":
                        DateTime dateOfBirth;
                        bool dateSuccess = DateTime.TryParseExact(conditions[i + 1], "MM'/'dd'/'yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateOfBirth);
                        if (dateSuccess)
                        {
                            if (predicate1 == null)
                            {
                                predicate1 = record => record.DateOfBirth == dateOfBirth;
                            }
                            else
                            {
                                predicate2 = record => record.DateOfBirth == dateOfBirth;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Can't extract dateofbirth. You typed invalid symbols!");
                        }

                        continue;
                }
            }

            firstPredicate = predicate1;
            secondPredicate = predicate2;
        }
    }
}
