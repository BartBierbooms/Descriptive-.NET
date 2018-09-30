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
	public abstract class Option<TV, TS> : PipeBase<TV, TS>
		where TV : new()
		where TS : new()
	{
		protected internal bool? ConditionMet;
		protected internal readonly IList<IValueAndSupplementExtension> ExtensionInterfaces;
		protected abstract TS GetSupplement { get; }
		protected abstract TV GetValue { get; }

		protected abstract int PipeType { get; }
		protected override bool? ConditionIsMet => ConditionMet;
		protected override IList<IValueAndSupplementExtension> PipebaseExtensions => ExtensionInterfaces;

		public OptionType GetOptionType => (OptionType)PipeType;

		public override bool TestInputIsInvariant<TI>(TI input)
		{
			return (input != null && input is IInvariant<TI> && !((IInvariant<TI>)input).IsValid);
		}

		public override bool ValidateIsValid<TA, TB>(PipeBase<TA, TB> source, out PipeBase<TA, TB> pipeBaseValidated)
		{
			pipeBaseValidated = source;
			if (source.Val != null && source.Val is IInvariant<TA> && !((IInvariant<TA>)source.Val).IsValid)
			{
				pipeBaseValidated = new Validation<TA, TB>(new ValueAndSupplement<TA, TB>(source.Val, source.SupplementVal), new PipeOption(null, null), ((IInvariant<TV>)source.Val).ToValidationResult());
				return false;
			}
			if (source.SupplementVal != null && source.SupplementVal is IInvariant<TS> && !((IInvariant<TS>)source.SupplementVal).IsValid)
			{
				pipeBaseValidated = new Validation<TA, TB>(new ValueAndSupplement<TA, TB>(source.Val, source.SupplementVal), new PipeOption(null, null), ((IInvariant<TS>)source.SupplementVal).ToValidationResult());
				return false;
			}
			return true;
		}

		public override PipeBase<TV, TS> CreateValidation(IValueAndSupplement<TV, TS> val, IPipeOption pipeOption, ValidationResult result)
		{
			return new Validation<TV, TS>(val, pipeOption, result);
		}

		public override PipeBase<TA, TB> CreateException<TA, TB>(Exception ex, IPipeOption pipeOption)
		{
			return new SomeException<TA, TB>(ex, pipeOption);
		}
		public override PipeBase<TV, TS> CreateNone(IPipeOption pipeOption)
		{
			return new None<TV, TS>(pipeOption);
		}
		public override PipeBase<TV, TS> CreateSome(IValueAndSupplement<TV, TS> val, IPipeOption pipeOption)
		{
			return new Some<TV, TS>(val, pipeOption);

		}

		public override PipeBase<TA, TB> WrapPipeLineResult<TA, TB>(TA value, TB supplementedValue, IPipeOption pipeOption)
		{
			if (value == null && supplementedValue == null)
			{
				return new SomeException<TA, TB>(new InvalidOperationException("Null values for value and supplemented value are not allowed!"), pipeOption);
			}
			if (supplementedValue == null)
			{
				return new SomeException<TA, TB>(new InvalidOperationException("Null values for supplemented value is not allowed!"), pipeOption);
			}

			try
			{
				ExecuteExtensions(pipeOption, value);
				ExecuteExtensions(pipeOption, supplementedValue);
			}
			catch (Exception ex)
			{
				return new SomeException<TA, TB>(ex, pipeOption);
			}
			return new Some<TA, TB>(new ValueAndSupplement<TA, TB>(value, supplementedValue), pipeOption);
		}

		public override PipeBase<TI, TA> WrapPipeLineResult<TI, TA>(Func<TA> init, TI value, PipeOption pipeOption, bool isInit = false)
		{
			TA nwevalue = default(TA);

			try
			{
				nwevalue = init();
				if (isInit && value.Equals(nwevalue))
				{
					return new SomeException<TI, TA>(new InvalidOperationException("The Init function must create a new instance of an object"), pipeOption);
				}
				ExecuteExtensions(pipeOption, value);
				ExecuteExtensions(pipeOption, nwevalue);
			}
			catch (Exception ex)
			{
				return new SomeException<TI, TA>(ex, pipeOption);
			}
			if (nwevalue == null)
			{
				return new None<TI, TA>(pipeOption);
			}
			return new Some<TI, TA>(new ValueAndSupplement<TI, TA>(value, nwevalue), pipeOption);

		}

		public override bool ContinuePipeLineEntry<TA, TB, TC, TD>(
			IValueAndSupplement<TA, TB> pipeInput,
			out PipeBase<TC, TD> wrapInputIntoPipeBaseWhenBreak,
			out IPipeOption pipeOption			)
		{
			//Determine pipeBaseWhenContinue
			if (pipeInput == null)
			{
				pipeOption = new PipeOption(null, null);
			}
			else
			{
				pipeOption = (IPipeOption)pipeInput;
			}


			//Determine pipeBaseWhenBreak
			if (pipeInput.GetType() == typeof(Option<TA, TB>))
			{
				wrapInputIntoPipeBaseWhenBreak = new None<TC, TD>(pipeOption);
				return false;
			}
			if (pipeInput.GetType() == typeof(SomeException<TA, TB>))
			{
				wrapInputIntoPipeBaseWhenBreak = new SomeException<TC, TD>(((SomeException<TA, TB>)pipeInput).ExceptionVal, pipeOption);
				return false;
			}

			if (pipeInput.GetType() == typeof(Validation<TA, TB>))
			{
				wrapInputIntoPipeBaseWhenBreak = new Validation<TC, TD>(new ValueAndSupplement<TC, TD>(default(TC), default(TD)), pipeOption, ((Validation<TC, TD>)pipeInput).ValidationRet);
				return false;
			}

			//Because of the out signature we need to instantiate a pipeBaseWhenBreak, but it will be ignored because we return true, meaning don't break
			wrapInputIntoPipeBaseWhenBreak = new Some<TC, TD>(new ValueAndSupplement<TC, TD>(default(TC), default(TD)), pipeOption);
			return true;
		}

		public override bool ContinuePipeLineEntry<TB, TA>(
			IValueAndSupplement<TA, TB> pipeInput,
			out PipeBase<TB, TA> wrapInputIntoPipeBase,
			out IPipeOption pipeOption)
		{
			if (pipeInput == null)
			{
				pipeOption = new PipeOption(null, null);
				wrapInputIntoPipeBase = new None<TB, TA>(pipeOption);
				return false;
			}
			pipeOption = (IPipeOption)pipeInput;

			if (pipeInput.GetType() == typeof(Option<TA, TB>))
			{
				wrapInputIntoPipeBase = new None<TB, TA>(pipeOption);
				return false;
			}
			if (pipeInput.GetType() == typeof(SomeException<TA, TB>))
			{
				wrapInputIntoPipeBase = new SomeException<TB, TA>(((SomeException<TA, TB>)pipeInput).ExceptionVal, pipeOption);
				return false;
			}
			if (pipeInput.GetType() == typeof(Validation<TA, TB>))
			{
				wrapInputIntoPipeBase = new Validation<TB, TA>(new ValueAndSupplement<TB, TA>(pipeInput.SupplementVal, pipeInput.Val), pipeOption, ((Validation<TB, TA>)pipeInput).ValidationRet);
				return false;
			}
			wrapInputIntoPipeBase = new Some<TB, TA>(new ValueAndSupplement<TB, TA>(pipeInput.SupplementVal, pipeInput.Val), pipeOption);
			return true;

		}

		public override bool ContinuePipeLineEntry<TA, TB>(
			IValueAndSupplement<TA, TB> pipeInput,
			out PipeBase<TA, TB> wrapInputIntoPipeBase,
			out IPipeOption pipeOption)
		{
			if (pipeInput == null)
			{
				pipeOption = new PipeOption(null, null);
				wrapInputIntoPipeBase = new None<TA, TB>(pipeOption);
				return false;
			}
			pipeOption = (IPipeOption)pipeInput;

			if (pipeInput.GetType() == typeof(Option<TA, TB>))
			{
				wrapInputIntoPipeBase = new None<TA, TB>(pipeOption);
				return false;
			}
			if (pipeInput.GetType() == typeof(SomeException<TA, TB>))
			{
				wrapInputIntoPipeBase = new SomeException<TA, TB>(((SomeException<TA, TB>)pipeInput).ExceptionVal, pipeOption);
				return false;
			}
			if (pipeInput.GetType() == typeof(Validation<TA, TB>))
			{
				wrapInputIntoPipeBase = new Validation<TA, TB>(new ValueAndSupplement<TA, TB>(pipeInput.Val, pipeInput.SupplementVal), pipeOption, ((Validation<TV, TS>)pipeInput).ValidationRet);
				return false;
			}

			wrapInputIntoPipeBase = new Some<TA, TB>(new ValueAndSupplement<TA, TB>(pipeInput.Val, pipeInput.SupplementVal), pipeOption);
			return true;
		}

		internal Option(IPipeOption pipeOption)
		{
			ConditionMet = pipeOption?.ConditionIsMet;
			ExtensionInterfaces = pipeOption?.Extensions;
		}


		override public TS SupplementVal
		{
			get
			{
				switch ((OptionType)this.PipeType)
				{
					case OptionType.Exception:
						return ((SomeException<TV, TS>)this).GetSupplement;
					case OptionType.None:
						return ((None<TV, TS>)this).GetSupplement;
					case OptionType.Some:
						return ((Some<TV, TS>)this).GetSupplement;
					case OptionType.Validation:
						return ((Validation<TV, TS>)this).GetSupplement;
				}
				throw new InvalidOperationException();
			}
		}

		override public TV Val
		{
			get
			{
				switch ((OptionType)this.PipeType)
				{
					case OptionType.Exception:
						return ((SomeException<TV, TS>)this).GetValue;
					case OptionType.None:
						return ((None<TV, TS>)this).GetValue;
					case OptionType.Some:
						return ((Some<TV, TS>)this).GetValue;
					case OptionType.Validation:
						return ((Validation<TV, TS>)this).GetValue;
				}
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

	}

	public class Some<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
		where TV : new()
		where TS : new()
	{
		public readonly IValueAndSupplement<TV, TS> SomeValAndSupplement;

		public Some(IValueAndSupplement<TV, TS> val, IPipeOption pipeOption) : base(pipeOption)
		{
			SomeValAndSupplement = val;
		}
		protected override TS GetSupplement => SomeValAndSupplement.SupplementVal;
		protected override TV GetValue => SomeValAndSupplement.Val;
		protected override int PipeType => (int)OptionType.Some;
	}

	public class Validation<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
		where TV : new()
		where TS : new()
	{
		public readonly IValueAndSupplement<TV, TS> SomeValAndSupplement;
		public readonly ValidationResult ValidationRet;

		public Validation(IValueAndSupplement<TV, TS> val, IPipeOption pipeOption, ValidationResult validationResult) : base(pipeOption)
		{
			SomeValAndSupplement = val;
			ValidationRet = validationResult;
		}

		protected override TS GetSupplement => SomeValAndSupplement.SupplementVal;
		protected override TV GetValue => SomeValAndSupplement.Val;
		protected override int PipeType => (int)OptionType.Validation;
	}
	public class None<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
		where TV : new()
		where TS : new()
	{
		public None(IPipeOption pipeOption) : base(pipeOption)
		{
		}

		protected override TS GetSupplement => default(TS);
		protected override TV GetValue => default(TV);
		protected override int PipeType => (int)OptionType.None;
	}

	public class SomeException<TV, TS> : Option<TV, TS>, IValueAndSupplement<TV, TS>
		where TV : new()
		where TS : new()
	{
		new public readonly Exception ExceptionVal;

		public SomeException(Exception val, IPipeOption pipeOption) : base(pipeOption)
		{
			ExceptionVal = val;
		}
		protected override TS GetSupplement => default(TS);
		protected override TV GetValue => default(TV);
		protected override int PipeType => (int)OptionType.Exception;
	}
}
