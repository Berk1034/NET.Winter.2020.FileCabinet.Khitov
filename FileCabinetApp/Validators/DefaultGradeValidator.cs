using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class DefaultGradeValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Grade < ValidationRules.DefaultMinGradeInPoints || recordInfo.Grade > ValidationRules.DefaultMaxGradeInPoints)
            {
                throw new ArgumentException($"Grade should be in range [{ValidationRules.DefaultMinGradeInPoints};{ValidationRules.DefaultMaxGradeInPoints}].", nameof(recordInfo.Grade));
            }
        }
    }
}
