using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class CustomGradeValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord recordInfo)
        {
            if (recordInfo.Grade < ValidationRules.CustomMinGradeInPoints || recordInfo.Grade > ValidationRules.CustomMaxGradeInPoints)
            {
                throw new ArgumentException($"Grade should be in range [{ValidationRules.CustomMinGradeInPoints};{ValidationRules.CustomMaxGradeInPoints}].", nameof(recordInfo.Grade));
            }
        }
    }
}
