using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FileCabinetApp;
using FileCabinetApp.Validators;

namespace FileCabinetGenerator
{
    public static class Program
    {
        private static int startId;
        private static int recordsAmount;
        private static string outputFileName;
        private static IRecordValidator recordValidator = new DefaultValidator();
        private static Random random = new Random(Environment.TickCount);
        public static void Main(string[] args)
        {
            string outputType = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--output-type=", StringComparison.OrdinalIgnoreCase))
                {
                    if (args[i].Remove(0, "--output-type=".Length).ToLower(null) == "csv")
                    {
                        outputType = "csv";
                    }
                    else if (args[i].Remove(0, "--output-type=".Length).ToLower(null) == "xml")
                    {
                        outputType = "xml";
                    }
                }

                if (args[i] == "-t")
                {
                    if (args[i + 1].ToLower(null) == "csv")
                    {
                        outputType = "csv";
                    }
                    else if (args[i + 1].ToLower(null) == "xml")
                    {
                        outputType = "xml";
                    }
                }

                if (args[i].Contains("--output=", StringComparison.OrdinalIgnoreCase))
                {
                    outputFileName = args[i].Remove(0, "--output=".Length);
                }

                if (args[i] == "-o")
                {
                    outputFileName = args[i + 1];
                }

                if (args[i].Contains("--records-amount=", StringComparison.OrdinalIgnoreCase))
                {
                    recordsAmount = Convert.ToInt32(args[i].Remove(0, "--records-amount=".Length));
                }

                if (args[i] == "-a")
                {
                    recordsAmount = Convert.ToInt32(args[i + 1]);
                }

                if (args[i].Contains("--start-id=", StringComparison.OrdinalIgnoreCase))
                {
                    startId = Convert.ToInt32(args[i].Remove(0, "--start-id=".Length));
                }

                if (args[i] == "-i")
                {
                    startId = Convert.ToInt32(args[i + 1]);
                }
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            for (int i = 0; i < recordsAmount; i++)
            {
                var record = new FileCabinetRecord()
                {
                    Id = startId,
                    Name = new Name 
                    { 
                        FirstName = random.NextString(recordValidator.MinLength, recordValidator.MaxLength),
                        LastName = random.NextString(recordValidator.MinLength, recordValidator.MaxLength),
                    },
                    DateOfBirth = random.NextDateTime(recordValidator.MinimalDate, recordValidator.MaximalDate),
                    Height = random.NextDecimal(recordValidator.MinHeight, recordValidator.MaxHeight),
                    Grade = random.NextShort(recordValidator.MinGrade, recordValidator.MaxGrade),
                    FavouriteSymbol = random.NextChar(recordValidator.ExcludeChar),
                };
                startId++;
                records.Add(record);
            }

            switch (outputType)
            {
                case "csv":
                    ExportToCsv(records);
                    Console.WriteLine($"{recordsAmount} records were written to {outputFileName}");
                    break;
                case "xml":
                    ExportToXml(records);
                    Console.WriteLine($"{recordsAmount} records were written to {outputFileName}");
                    break;
                default:
                    break;
            }
        }
       
        public static string NextString(this Random rnd, int from, int to)
        {
            string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] resultChars = new char[to];
            int length = rnd.Next(from, to + 1);
            for (int i = 0; i < length; i++)
            {
                resultChars[i] = allowedChars[rnd.Next(allowedChars.Length)];
            }

            return new string(resultChars, 0, length);
        }
        
        public static DateTime NextDateTime(this Random rnd, DateTime from, DateTime to)
        {
            var range = to - from;

            var randomisedTimeSpan = new TimeSpan(Convert.ToInt64(rnd.NextDouble() * range.Ticks));

            return (from + randomisedTimeSpan).Date;
        }
        
        public static char NextChar(this Random rnd, char exclude)
        {
            char result = Convert.ToChar(rnd.Next(32, 127 + 1));
            if (result == recordValidator.ExcludeChar)
            {
                result = Convert.ToChar(Convert.ToInt32(exclude) + 10);
            }

            return result;
        }

        public static short NextShort(this Random rnd, short from, short to)
        {
            return Convert.ToInt16(rnd.Next(from, to + 1)); 
        }

        public static decimal NextDecimal(this Random rnd, decimal from, decimal to)
        {
            byte fromScale = new System.Data.SqlTypes.SqlDecimal(from).Scale;
            byte toScale = new System.Data.SqlTypes.SqlDecimal(to).Scale;

            byte scale = Convert.ToByte(fromScale + toScale);
            if (scale > 28)
            {
                scale = 28;
            }

            decimal result = new decimal(rnd.Next(), rnd.Next(), rnd.Next(), false, scale);
            if (Math.Sign(from) == Math.Sign(to) || from == 0 || to == 0)
            {
                return decimal.Remainder(result, to - from) + from;
            }

            return decimal.Remainder(result, to);
        }

        private static void ExportToCsv(List<FileCabinetRecord> records)
        {
            using (TextWriter textWriter = new StreamWriter(outputFileName))
            {
                textWriter.WriteLine("Id,First Name,Last Name,Date of birth,Grade,Height,Favourite symbol");
                foreach (var record in records)
                {
                    textWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6}", record.Id, record.Name.FirstName, record.Name.LastName, record.DateOfBirth.ToString("MM'/'dd'/'yyyy", null), record.Grade, record.Height, record.FavouriteSymbol);
                }
            }
        }

        private static void ExportToXml(List<FileCabinetRecord> records)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<FileCabinetRecord>), new XmlRootAttribute("records"));
            using (StreamWriter streamWriter = new StreamWriter(outputFileName, false, Encoding.UTF8))
            {
                formatter.Serialize(streamWriter, records);
            }
        }
    }
}
