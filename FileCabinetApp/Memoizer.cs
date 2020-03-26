using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// The Memoizer class.
    /// </summary>
    public class Memoizer
    {
        private Dictionary<string, IEnumerable<FileCabinetRecord>> cacheOfRecords = new Dictionary<string, IEnumerable<FileCabinetRecord>>();

        /// <summary>
        /// Adds records to the cacheOfRecords.
        /// </summary>
        /// <param name="rawKey">The raw key.</param>
        /// <param name="records">The records to add.</param>
        public void Memoize(string[] rawKey, IEnumerable<FileCabinetRecord> records)
        {
            string generatedKey = this.GenerateKey(rawKey);
            this.cacheOfRecords.Add(generatedKey, records);
        }

        /// <summary>
        /// Tries to retrieve cached records by the key.
        /// </summary>
        /// <param name="rawKey">The raw key.</param>
        /// <param name="cachedRecords">The cached records.</param>
        /// <returns>True if cacheOfRecords contains key generated from raw key, otherwise false.</returns>
        public bool TryGetCachedRecords(string[] rawKey, out IEnumerable<FileCabinetRecord> cachedRecords)
        {
            cachedRecords = null;

            if (!this.cacheOfRecords.TryGetValue(this.GenerateKey(rawKey), out cachedRecords))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears the cacheOfRecords.
        /// </summary>
        public void Clear()
        {
            this.cacheOfRecords.Clear();
        }

        private string GenerateKey(string[] rawKey)
        {
            StringBuilder generatedKey = new StringBuilder();
            generatedKey.Append("%");
            foreach (var str in rawKey)
            {
                generatedKey.Append(str + "%");
            }

            return generatedKey.ToString();
        }
    }
}
