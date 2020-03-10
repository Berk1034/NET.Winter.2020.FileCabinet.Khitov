using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class GradeValidator : IRecordValidator
    {
        private short minGrade;
        private short maxGrade;

        public GradeValidator(short minGrade, short maxGrade)
        {
            this.minGrade = minGrade;
            this.maxGrade = maxGrade;
        }

        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Grade < this.minGrade || recordInfo.Grade > this.maxGrade)
            {
                throw new ArgumentException($"Grade should be in range [{this.minGrade};{this.maxGrade}].", nameof(recordInfo.Grade));
            }
        }
    }
}
