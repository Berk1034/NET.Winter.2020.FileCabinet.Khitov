using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    public class FilesystemIterator : IRecordIterator
    {
        private const int ReservedSizeInBytes = 2;
        private const int StringSizeInBytes = 120;
        private readonly long[] offsets;
        private BinaryReader reader;
        private int currentPosition;

        public FilesystemIterator(BinaryReader binaryReader, long[] offsets)
        {
            this.reader = binaryReader;
            this.currentPosition = -1;
            this.offsets = offsets;
        }

        public FileCabinetRecord GetNext()
        {
            if (this.currentPosition == -1 || this.currentPosition == this.offsets.Length)
            {
                throw new InvalidOperationException("No current item.");
            }

            long recordOffset = this.offsets[this.currentPosition];
            this.reader.BaseStream.Seek(recordOffset, SeekOrigin.Begin);
            this.reader.BaseStream.Seek(ReservedSizeInBytes, SeekOrigin.Current);
            int id = this.reader.ReadInt32();
            string firstName = this.reader.ReadString();
            this.reader.BaseStream.Seek(StringSizeInBytes - firstName.Length - 1, SeekOrigin.Current);
            string lastName = this.reader.ReadString();
            this.reader.BaseStream.Seek(StringSizeInBytes - lastName.Length - 1, SeekOrigin.Current);
            int year = this.reader.ReadInt32();
            int month = this.reader.ReadInt32();
            int day = this.reader.ReadInt32();
            short grade = this.reader.ReadInt16();
            decimal height = this.reader.ReadDecimal();
            char favouriteSymbol = this.reader.ReadChar();

            var record = new FileCabinetRecord()
            {
                Id = id,
                Name = new Name
                {
                    FirstName = firstName,
                    LastName = lastName,
                },
                DateOfBirth = new DateTime(year, month, day),
                Grade = grade,
                Height = height,
                FavouriteSymbol = favouriteSymbol,
            };

            return record;
        }

        public bool HasMore()
        {
            var result = ++this.currentPosition < this.offsets.Length;
            if (!result)
            {
                this.reader.Close();
            }

            return result;
        }
    }
}
