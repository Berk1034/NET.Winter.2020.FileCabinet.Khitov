using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The SelectCommandHandler class.
    /// </summary>
    public class SelectCommandHanlder : ServiceCommandHandlerBase
    {
        private IPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHanlder"/> class.
        /// </summary>
        /// <param name="service">The IFileCabinetService service.</param>
        /// <param name="printer">The IRecordPrinter printer.</param>
        public SelectCommandHanlder(IFileCabinetService service, IPrinter printer)
            : base(service)
        {
            this.printer = printer;
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

            if (appCommandRequest.Command == "select")
            {
                try
                {
                    char[] separators = { ',', ' ', '=' };
                    string[] sourceInput = appCommandRequest.Parameters.Split("where", StringSplitOptions.RemoveEmptyEntries);

                    string[] recordFields = sourceInput[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (recordFields.Length == 0)
                    {
                        throw new ArgumentException("Not enough parameters after 'select' command.");
                    }

                    for (int i = 0; i < recordFields.Length; i++)
                    {
                        recordFields[i] = recordFields[i].ToLower(null);
                        switch (recordFields[i])
                        {
                            case "id":
                                recordFields[i] = "Id";
                                break;
                            case "firstname":
                                recordFields[i] = "FirstName";
                                break;
                            case "lastname":
                                recordFields[i] = "LastName";
                                break;
                            case "dateofbirth":
                                recordFields[i] = "DateOfBirth";
                                break;
                            case "grade":
                                recordFields[i] = "Grade";
                                break;
                            case "height":
                                recordFields[i] = "Height";
                                break;
                            case "favouritesymbol":
                                recordFields[i] = "FavouriteSymbol";
                                break;
                            case "*":
                                break;
                            default:
                                recordFields[i] = string.Empty;
                                break;
                        }
                    }

                    recordFields = recordFields.Where(field => !string.IsNullOrEmpty(field)).ToArray();
                    if (recordFields.Length == 0)
                    {
                        throw new ArgumentException("No such fields.");
                    }

                    if (sourceInput.Length == 1)
                    {
                        if (recordFields.Length == 1 && recordFields[0] == "*")
                        {
                            recordFields = new string[7] { "Id", "FirstName", "LastName", "DateOfBirth", "Grade", "Height", "FavouriteSymbol" };
                        }

                        List<FileCabinetRecord> recordsMatch = new List<FileCabinetRecord>(this.Service.GetRecords());
                        if (recordsMatch.Count != 0)
                        {
                            this.printer.Print(recordsMatch, recordFields);
                        }
                        else
                        {
                            Console.WriteLine($"No records found.");
                        }
                    }
                    else
                    {
                        string[] conditions = sourceInput[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        if (conditions.Length > 2 && !conditions.Contains("and") && !conditions.Contains("or"))
                        {
                            throw new ArgumentException("No separators 'and/or' after parameters!");
                        }

                        if (conditions.Contains("and") && conditions.Contains("or"))
                        {
                            throw new ArgumentException("Can't contain both 'and/or' separators.");
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

                        if (Memoizer.TryGetCachedRecords(conditions, out var cachedRecords))
                        {
                            this.printer.Print(cachedRecords, recordFields);
                            return;
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

                        if (recordsMatch.Count != 0)
                        {
                            Memoizer.Memoize(conditions, recordsMatch);
                            this.printer.Print(recordsMatch, recordFields);
                        }
                        else
                        {
                            Console.WriteLine($"No records with such criteria found.");
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Enter the fields to select.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't select records.");
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
