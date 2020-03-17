using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The IRecordIterator interface.
    /// </summary>
    public interface IRecordIterator
    {
        FileCabinetRecord GetNext();

        bool HasMore();
    }
}
