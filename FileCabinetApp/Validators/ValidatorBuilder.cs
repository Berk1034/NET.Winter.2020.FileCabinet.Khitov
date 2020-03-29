using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The ValidatorBuilder class.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Adds new instance of FirstNameValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="min">The minimal length for the firstname.</param>
        /// <param name="max">The maximal length for the firstname.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds new instance of LastNameValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="min">The minimal length for the lastname.</param>
        /// <param name="max">The maximal length for the lastname.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds new instance of DateOfBirthValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="from">The minimal date for the dateofbirth.</param>
        /// <param name="to">The maximal date for the dateofbirth.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds new instance of GradeValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="min">The minimal value for the grade.</param>
        /// <param name="max">The maximal value for the grade.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateGrade(short min, short max)
        {
            this.validators.Add(new GradeValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds new instance of HeightValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="min">The minimal value for the height.</param>
        /// <param name="max">The maximal value for the height.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateHeight(decimal min, decimal max)
        {
            this.validators.Add(new HeightValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds new instance of FavouriteSymbolValidator with parameters to the list of validators.
        /// </summary>
        /// <param name="bannedSymbol">The symbol to ban for favouritesymbol.</param>
        /// <returns>The reference to the current instance of the ValidatorBuilder class.</returns>
        public ValidatorBuilder ValidateFavouriteSymbol(char bannedSymbol)
        {
            this.validators.Add(new FavouriteSymbolValidator(bannedSymbol));
            return this;
        }

        /// <summary>
        /// Creates the new instance of CompositeValidator with added validators.
        /// </summary>
        /// <returns>The IRecordValidator reference.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
