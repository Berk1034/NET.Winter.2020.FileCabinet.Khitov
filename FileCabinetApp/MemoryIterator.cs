using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class MemoryIterator : IRecordIterator
    {
        private readonly FileCabinetRecord[] records;
        private int currentPosition;

        public MemoryIterator(FileCabinetRecord[] records)
        {
            this.currentPosition = -1;
            this.records = records;
        }

        public FileCabinetRecord GetNext()
        {
            if (this.currentPosition == -1 || this.currentPosition == this.records.Length)
            {
                throw new InvalidOperationException("No current item.");
            }

            return this.records[this.currentPosition];
        }

        public bool HasMore()
        {
            return ++this.currentPosition < this.records.Length;
        }
    }
}
