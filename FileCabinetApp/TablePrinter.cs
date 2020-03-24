using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The TablePrinter class.
    /// </summary>
    public class TablePrinter : IPrinter
    {
        private const string Connector = "+";
        private const string HorizontalLine = "-";
        private const string VerticalLine = "|";

        /// <summary>
        /// Prints the records with specified fields in table view.
        /// </summary>
        /// <param name="records">The records to be printed.</param>
        /// <param name="recordFields">The fields of records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records, string[] recordFields)
        {
            List<int> fieldsLength = GetFieldsMaxLength(records, recordFields);
            string horizontalBoarder = CreateHorizontalBoarder(records, recordFields, fieldsLength);
            Console.WriteLine(horizontalBoarder);
            Console.WriteLine(CreateHead(recordFields, fieldsLength));
            Console.WriteLine(horizontalBoarder);

            foreach (var record in records)
            {
                Console.WriteLine(CreateRecordLine(record, recordFields, fieldsLength));
                Console.WriteLine(horizontalBoarder);
            }
        }

        private static string CreateHorizontalBoarder(IEnumerable<FileCabinetRecord> records, string[] recordFields, List<int> fieldsLength)
        {
            StringBuilder horizontalBoarder = new StringBuilder();
            horizontalBoarder.Append(Connector + HorizontalLine);

            for (int i = 0; i < recordFields.Length; i++)
            {
                int recordFieldMaxLength = fieldsLength[i];
                for (int j = 0; j < recordFieldMaxLength; j++)
                {
                    horizontalBoarder.Append(HorizontalLine);
                }

                if (i < recordFields.Length - 1)
                {
                    horizontalBoarder.Append(HorizontalLine + Connector + HorizontalLine);
                }
                else
                {
                    horizontalBoarder.Append(HorizontalLine + Connector);
                }
            }

            return horizontalBoarder.ToString();
        }

        private static List<int> GetFieldsMaxLength(IEnumerable<FileCabinetRecord> records, string[] recordFields)
        {
            List<int> fieldsMaxLength = new List<int>();
            foreach (var recordField in recordFields)
            {
                int fieldMaxLength = recordField.Length;
                foreach (var record in records)
                {
                    int recordLength;
                    object recordValue = null;

                    switch (recordField)
                    {
                        case "Id":
                            recordValue = record.Id;
                            break;
                        case "FirstName":
                            recordValue = record.Name.FirstName;
                            break;
                        case "LastName":
                            recordValue = record.Name.LastName;
                            break;
                        case "DateOfBirth":
                            recordValue = record.DateOfBirth;
                            break;
                        case "Grade":
                            recordValue = record.Grade;
                            break;
                        case "Height":
                            recordValue = record.Height;
                            break;
                        case "FavouriteSymbol":
                            recordValue = record.FavouriteSymbol;
                            break;
                    }

                    if (recordValue is null)
                    {
                        continue;
                    }

                    if (recordValue is DateTime)
                    {
                        recordLength = ((DateTime)recordValue).ToString("yyyy-MMM-dd", new CultureInfo("en-US")).Length;
                    }
                    else
                    {
                        recordLength = recordValue.ToString().Length;
                    }

                    if (recordLength > fieldMaxLength)
                    {
                        fieldMaxLength = recordLength;
                    }
                }

                fieldsMaxLength.Add(fieldMaxLength);
            }

            return fieldsMaxLength;
        }

        private static string CreateHead(string[] recordFields, List<int> fieldsLength)
        {
            StringBuilder head = new StringBuilder();
            head.Append(VerticalLine);
            for (int i = 0; i < recordFields.Length; i++)
            {
                head.Append($" {recordFields[i].PadRight(fieldsLength[i])} ");
                head.Append(VerticalLine);
            }

            return head.ToString();
        }

        private static string CreateRecordLine(FileCabinetRecord record, string[] recordFields, List<int> fieldsLength)
        {
            StringBuilder recordLine = new StringBuilder();
            recordLine.Append(VerticalLine);
            for (int i = 0; i < recordFields.Length; i++)
            {
                object recordValue = null;

                switch (recordFields[i])
                {
                    case "Id":
                        recordValue = record.Id;
                        break;
                    case "FirstName":
                        recordValue = record.Name.FirstName;
                        break;
                    case "LastName":
                        recordValue = record.Name.LastName;
                        break;
                    case "DateOfBirth":
                        recordValue = record.DateOfBirth;
                        break;
                    case "Grade":
                        recordValue = record.Grade;
                        break;
                    case "Height":
                        recordValue = record.Height;
                        break;
                    case "FavouriteSymbol":
                        recordValue = record.FavouriteSymbol;
                        break;
                }

                if (recordValue is null)
                {
                    continue;
                }

                if (recordValue is string || recordValue is char)
                {
                    recordLine.Append($" {recordValue.ToString().PadRight(fieldsLength[i])} ");
                    recordLine.Append(VerticalLine);
                }
                else if (recordValue is DateTime)
                {
                    recordLine.Append($" {((DateTime)recordValue).ToString("yyyy-MMM-dd", new CultureInfo("en-US")).PadLeft(fieldsLength[i])} ");
                    recordLine.Append(VerticalLine);
                }
                else
                {
                    recordLine.Append($" {recordValue.ToString().PadLeft(fieldsLength[i])} ");
                    recordLine.Append(VerticalLine);
                }
            }

            return recordLine.ToString();
        }
    }
}
