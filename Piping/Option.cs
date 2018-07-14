using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piping
{

    public enum OptionType
    {
        None = 0,
        Some = 1,
        Exception = 2,
        Validation = 3
    }

    /// <summary>
    /// Abstract class with different pattern matching implementations. 
    /// The returned value of an invoked pipeline method is always(!) one of the defined implementations.
    ///    Returned null values are wrapped into a None option, Exceptions are wrapped in SomeException option.
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TS"></typeparam>
    public abstract class Option<TV, TS> : IValueAndSupplement<TV, TS>, IPipeOption
    {
        protected internal bool? ConditionMet;
        protected internal readonly IList<IValueAndSupplementExtension> ExtensionInterfaces;

        bool? IPipeOption.ConditionIsMet => ConditionMet;

        IList<IValueAndSupplementExtension> IPipeOption.Extensions => ExtensionInterfaces;

        internal Option(IPipeOption pipeOption)
        {
            ConditionMet = pipeOption.ConditionIsMet;
            ExtensionInterfaces = pipeOption.Extensions;
        }

        protected abstract TS GetSupplement { get; }
        protected abstract TV GetValue { get; }

        public OptionType GetOptionType
        {
            get
            {
                if (GetType() == typeof(Some<TV, TS>))
                    return OptionType.Some;
                if (GetType() == typeof(None<TV, TS>))
                    return OptionType.None;
                if (GetType() == typeof(SomeException<TV, TS>))
                    return OptionType.Exception;
                if (GetType() == typeof(Validation<TV, TS>))
                    return OptionType.Validation;
                throw new InvalidOperationException();
            }

        }

        public TS SupplementVal
        {
            get
            {
                if (GetType() == typeof(Some<TV, TS>))
                    return ((Some<TV, TS>)this).GetSupplement;
                if (GetType() == typeof(None<TV, TS>))
                    return ((None<TV, TS>)this).GetSupplement;
                if (GetType() == typeof(SomeException<TV, TS>))
                    return ((SomeException<TV, TS>)this).GetSupplement;
                if (GetType() == typeof(Validation<TV, TS>))
                    return ((Validation<TV, TS>)this).GetSupplement;
                throw new InvalidOperationException();
            }
        }

        public TV Val
        {
            get
            {
                if (GetType() == typeof(Some<TV, TS>))
                    return ((Some<TV, TS>)this).GetValue;
                if (GetType() == typeof(None<TV, TS>))
                    return ((None<TV, TS>)this).GetValue;
                if (GetType() == typeof(SomeException<TV, TS>))
                    return ((SomeException<TV, TS>)this).GetValue;
                if (GetType() == typeof(Validation<TV, TS>))
                    return ((Validation<TV, TS>)this).GetValue;
                throw new InvalidOperationException();
            }
        }

        public Exception ExceptionVal
        {
            get
            {
                if (this.GetType() == typeof(SomeException<TV, TS>))
                    return ((SomeException<TV, TS>)this).ExceptionVal;
                throw new InvalidOperationException();
            }
        }

        public ValidationResult ValidationResult
        {
            get
            {
                if (this.GetType() == typeof(Validation<TV, TS>))
                    return ((Validation<TV, TS>)this).ValidationRet;
                throw new InvalidOperationException();
            }
        }

        public static None<TV, TS> None(IPipeOption conditionMet) => new None<TV, TS>(conditionMet);
        public static Some<TV, TS> Some(ValueAndSupplement<TV, TS> pair, IPipeOption pipeOption) => new Some<TV, TS>(pair, pipeOption);
        public static SomeException<TV, TS> Exception(Exception ex, IPipeOption pipeOption) => new SomeException<TV, TS>(ex, pipeOption);
        public static Validation<TV, TS> SomeValidation(ValueAndSupplement<TV, TS> pair, IPipeOption pipeOption, ValidationResult validationResult) => new Validation<TV, TS>(pair, pipeOption, validationResult);


    }

    public class Some<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
    {
        public readonly ValueAndSupplement<TV, TS> SomeValAndSupplement;

        public Some(ValueAndSupplement<TV, TS> val, IPipeOption pipeOption) : base(pipeOption)
        {
            SomeValAndSupplement = val;
        }
        protected override TS GetSupplement => SomeValAndSupplement.SupplementVal;
        protected override TV GetValue => SomeValAndSupplement.Val;
    }

    public class Validation<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
    {
        public readonly ValueAndSupplement<TV, TS> SomeValAndSupplement;
        public readonly ValidationResult ValidationRet;

        public Validation(ValueAndSupplement<TV, TS> val, IPipeOption pipeOption, ValidationResult validationResult) : base(pipeOption)
        {
            SomeValAndSupplement = val;
            ValidationRet = validationResult;
        }

        protected override TS GetSupplement => SomeValAndSupplement.SupplementVal;
        protected override TV GetValue => SomeValAndSupplement.Val;

    }
    public class None<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
    {

        public None(IPipeOption pipeOption) : base(pipeOption)
        {
        }

        protected override TS GetSupplement => default(TS);
        protected override TV GetValue => default(TV);
    }

    public class SomeException<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
    {
        new public readonly Exception ExceptionVal;

        public SomeException(Exception val, IPipeOption pipeOption) : base(pipeOption)
        {
            ExceptionVal = val;
        }
        protected override TS GetSupplement => default(TS);
        protected override TV GetValue => default(TV);
    }
}
