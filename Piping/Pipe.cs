using System;
using System.Collections.Generic;

namespace Piping
{
	/// <summary>
	/// Class with extension methods on a ToValueSupplementValue delegate.
	///     The returned value are also ToValueSupplementValue delegates.
	///     That makes piping possible: The output of one call is the input of the next call.
	/// The ToValueSupplementValue delegate is a Func with an input parameter that must be supplied when invoked.
	///     When invoked, the returned type is an object that implements the IValueAndSupplement interface. Default this is an class derived from Option.
	///     Piping is constructing your definitions, which is separate from executing (invoking).
	/// </summary>
	public static class Pipe
	{
		public delegate IValueAndSupplement<TV, TS> ToValueSupplementValue<in TI, out TV, out TS>(TI i);

		#region init

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">Unit type (The type Unit has no implementation)</typeparam>
		/// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
		/// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
		/// <param name="execType">Optional. Default: Option type. The type of the object that is the input and output of the pipe segments when invoked.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		/// <example>
		/// <code>
		/// var pipelineDefine = Pipe.Init<Car, Unit>();
		/// var pipelineExec = pipelineDefine(new Car());
		/// </code>
		/// </example>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			IList<IValueAndSupplementExtension> postProcessings,
			PipeBase<TI, TV> execType = null)
			where TV : Unit, new()
			where TI : new()
		{
			return i =>
			{
				if (execType == null)
					execType = new Some<TI, TV>(new ValueAndSupplement<TI, TV>(default(TI), default(TV)), new PipeOption(null, null));

				Func<TV> CreateUnit() => () => (TV) new Unit();
				try
				{
					if (execType.TestInputIsInvariant(i))
					{
						return execType.CreateSome(new ValueAndSupplement<TI, TV>(i, CreateUnit()()), new PipeOption(null, null));
					}
				}
				catch (Exception ex)
				{
					return execType.CreateException<TI, TV>(ex, new PipeOption(null, null));
				}

				return execType.WrapPipeLineResult(CreateUnit(), i, new PipeOption(null, postProcessings), true);
			};
		}

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <param name="execType">Optional. Default: Option type. The type of the object that is the input and output of the pipe segments when invoked.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			PipeBase<TI, TV> execType = null)
			where TV : Unit, new()
			where TI : new()
		{
			return i =>
			{
				if (execType == null)
					execType = new Some<TI, TV>(new ValueAndSupplement<TI, TV>(default(TI), default(TV)), new PipeOption(null, null));

				Func<TV> CreateUnit() => () => (TV) new Unit();
				try
				{
					if (execType.TestInputIsInvariant(i))
					{
						return execType.CreateValidation(new ValueAndSupplement<TI, TV>(i, CreateUnit()()), new PipeOption(null, null), ((IInvariant<TI>) i).ToValidationResult());
					}
				}
				catch (Exception ex)
				{
					return execType.CreateException<TI, TV>(ex, new PipeOption(null, null));
				}

				return execType.WrapPipeLineResult(CreateUnit(), i, new PipeOption(null, null), true);
			};
		}

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <param name="init">Func delegate that returns an instance of type TV.</param>
		/// <param name="execType">Optional. Default: Option type. The type of the object that is the input and output of the pipe segments when invoked.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			Func<TV> init,
			PipeBase<TI, TV> execType = null
		)
			where TV : new()
			where TI : new()
		{
			return i =>
			{
				if (execType == null)
					execType = new Some<TI, TV>(new ValueAndSupplement<TI, TV>(default(TI), default(TV)), new PipeOption(null, null));

				try
				{
					if (execType.TestInputIsInvariant(i))
					{
						return execType.CreateValidation(new ValueAndSupplement<TI, TV>(i, default(TV)), new PipeOption(null, null), ((IInvariant<TI>) i).ToValidationResult());
					}
				}
				catch (Exception ex)
				{
					return execType.CreateException<TI, TV>(ex, new PipeOption(null, null));
				}

				return execType.WrapPipeLineResult(init, i, new PipeOption(null, null), true);
			};
		}

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <param name="init">Func delegate that creates an instance of type TV</param>
		/// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
		/// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
		/// <param name="execType">Optional. Default: Option type. The type of the object that is the input and output of the pipe segments when invoked.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			Func<TV> init, IList<IValueAndSupplementExtension> postProcessings,
			PipeBase<TI, TV> execType = null)
			where TV : new()
			where TI : new()
		{
			return i =>
			{
				try
				{
					if (execType == null)
						execType = new Some<TI, TV>(new ValueAndSupplement<TI, TV>(default(TI), default(TV)), new PipeOption(null, null));

					if (execType.TestInputIsInvariant(i))
					{
						return execType.CreateValidation(new ValueAndSupplement<TI, TV>(i, default(TV)), new PipeOption(null, null), ((IInvariant<TI>) i).ToValidationResult());
					}
				}
				catch (Exception ex)
				{
					return execType.CreateException<TI, TV>(ex, new PipeOption(null, null));
				}

				return execType.WrapPipeLineResult(init, i, new PipeOption(null, postProcessings), true);
			};
		}

		/// <summary>
		/// Usage: Init the pipe by providing another pipe that has the same signature.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <param name="init">Func delegate that creates an instance of type ToValueSupplementValue<TI, TV></param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an option that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			ToValueSupplementValue<TI, TI, TV> init)
			where TV : new()
			where TI : new()
		{
			return init;
		}

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <param name="init">Func delegate that takes an instance of type TI and returns an instance of type TV</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TI, TV> init)
			where TI : new()
			where TV : new()
		{
			return Init(init, null, null);
		}

		/// <summary>
		/// The input of a pipeline is an object to work on.
		/// You can initiate the pipeline with an (supplemented) object that works with the passed-in input when invoked.
		/// Example: The pipeline is initialized with a Validation object (its Supplement).
		///          The input of the pipeline (its Val) is a Car. The supplemented Validation object can be used to validate a specific instance of the car.
		///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <param name="init">Func delegate that takes an instance of type TI and returns an instance of type TV</param>
		/// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
		/// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
		/// <param name="execType">Optional. Default: Option type. The type of the object that is the input and output of the pipe segments when invoked.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(
			Func<TI, TV> init,
			IList<IValueAndSupplementExtension> postProcessings,
			PipeBase<TI, TV> execType = null)
			where TV : new()
			where TI : new()
		{
			return i =>
			{
				var pipeOption = new PipeOption(null, postProcessings);

				if (execType == null)
					execType = new Some<TI, TV>(new ValueAndSupplement<TI, TV>(default(TI), default(TV)), pipeOption);

				TV newValue = default(TV);

				try
				{
					if (execType.TestInputIsInvariant(i))
					{
						return execType.CreateValidation(new ValueAndSupplement<TI, TV>(i, newValue), new PipeOption(null, null), ((IInvariant<TI>) i).ToValidationResult());
					}

					newValue = init(i);
					if (i.Equals(newValue))
						return execType.CreateException<TI, TV>(new InvalidOperationException("The Init function must create a new instance of an object"), null);
				}
				catch (Exception ex)
				{
					return execType.CreateException<TI, TV>(ex, pipeOption);
				}

				if (newValue == null)
				{
					return execType.CreateNone(pipeOption);
				}

				return execType.CreateSome(new ValueAndSupplement<TI, TV>(i, newValue), pipeOption);
			};
		}

		#endregion

		#region TransForm

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type that is the Supplemented value of the source parameter.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Func delegate that takes the Supplemented value of the source and returns the Supplemented value of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TW> Transform<TI, TV, TW, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewValue)
			where TI : new()
			where TV : new()
			where TW : new()
			where TS : new()
		{
			return (i =>
			{
				TW newValue;
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput,
					out PipeBase<TV, TW> pipeBaseWhenBreak,
					out var pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					newValue = toNewValue(pipeInput.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TW>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(pipeInput.Val, newValue, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the returned out object of the ToValueSupplementValue delegate.</typeparam>
		/// <typeparam name="TS">The type was the Supplemented value of the source parameter.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Func delegate that takes the Val value of the source and returns the Val value of the returned IValueAndSupplement interface</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TS> Transform<TI, TV, TW, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TW> toNewValue)
			where TI : new()
			where TV : new()
			where TW : new()
			where TS : new()
		{
			return (i =>
			{
				TW newValue;
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TS> pipeBaseWhenBreak, out var pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					newValue = toNewValue(pipeInput.Val);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TS>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(newValue, pipeInput.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type that is the Supplemented value of the source parameter.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Func delegate that takes the Supplemented value of the source and returns the Val value of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <param name="toNewSupplementedValue">Func delegate that takes the returned value of toNewValue as input and returns the supplemented value of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewValue,
			Func<TR> toNewSupplementedValue)
			where TI : new()
			where TV : new()
			where TR : new()
			where TW : new()
			where TS : new()
		{
			var newConvertTo = Init<TW, TR>(toNewSupplementedValue);
			return source.Transform(toNewValue, newConvertTo, ((val, valAndExtension) => { }));
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Func delegate that takes the Val value of the source and returns the Val value of the returned IValueAndSupplement interface.</param>
		/// <param name="toNewSupplementedValue">Func delegate that takes the returned value of toNewValue as input and returns the Val value  of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TW> toNewValue,
			Func<TR> toNewSupplementedValue)
			where TI : new()
			where TV : new()
			where TR : new()
			where TW : new()
			where TS : new()
		{
			var newConvertTo = Init<TW, TR>(toNewSupplementedValue);
			return source.Transform(toNewValue, newConvertTo, ((TV val, TR valAndExtension) => { }));
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Functions that converts the initial val to a new val of another type that 
		/// serves as input parameter of the newInitValueSupplementedValue parameter.</param>
		/// <param name="newInitValueSupplementedValue"></param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TW> toNewValue,
			ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue)
			where TI : new()
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			return source.Transform(toNewValue, newInitValueSupplementedValue, (TV dummyTs, TR dummyTr) => { });
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Functions that converts the initial Supplemented value to a new val of another type that 
		/// serves as input parameter of the newInitValueSupplementedValue parameter.</param >
		/// <param name="newInitValueSupplementedValue"></param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewValue,
			ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue)
			where TI : new()
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			return source.Transform(toNewValue, newInitValueSupplementedValue, (TS dummyTs, TR dummyTr) => { });
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewSupplementedValue">Func delegate that takes the returned value of toNewValue as input and returns the supplemented value of the returned IValueAndSupplement interface</param>
		/// <param name="newInitValueSupplementedValue"></param>
		/// <param name="onNewSupplementedValue">Action delegate that takes the original Supplemented object of the source and the Supplemented val of the object that is returned.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Then<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewSupplementedValue, ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue,
			Action<TS, TR> onNewSupplementedValue)
			where TI : new()
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			return source.Transform(toNewSupplementedValue, newInitValueSupplementedValue, onNewSupplementedValue);
		}


		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Func delegate that takes the val of the source parameter and outputs a new type as val parameter of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <param name="newValueAndSupplement">parameter that takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
		/// <param name="onValue">Action delegate that takes the original Supplemented object of the source and the Supplemented val of the returned out object of the ToValueSupplementValue delegate.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TW> toNewValue,
			ToValueSupplementValue<TW, TW, TR> newValueAndSupplement,
			Action<TS, TR> onValue)
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			IPipeOption pipeOption;
			return (i =>
			{
				IValueAndSupplement<TW, TR> toNewValueState;

				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TR> pipeBaseWhenBreak, out pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					var newValue = toNewValue(pipeInput.Val);
					toNewValueState = newValueAndSupplement(newValue);
					onValue?.Invoke(pipeInput.SupplementVal, toNewValueState.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TR>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TS">The type that is the Supplemented value of the source parameter.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue"></param>
		/// <param name="newValueAndSupplement"></param>
		/// <param name="onValue"></param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TW> toNewValue, ToValueSupplementValue<TW, TW, TR> newValueAndSupplement,
			Action<TV, TR> onValue)
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			return (i =>
			{
				IValueAndSupplement<TW, TR> toNewValueState;

				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TR> pipeBaseWhenBreak, out var pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					var newValue = toNewValue(pipeInput.Val);
					toNewValueState = newValueAndSupplement(newValue);
					onValue?.Invoke(pipeInput.Val, toNewValueState.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TR>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Functions that converts the initial supplemented val to a new val of another type that 
		/// serves as input parameter of the newInitValueSupplementedValue parameter.</param>
		/// <param name="newInitValueSupplementedValue">parameter that takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
		/// <param name="onValue">Action that takes the initial supplemented  of the toNewValue parameter and the newly created Supplemented value of instance created by the newInitValueSupplementedValue parameter.
		/// The action can be used to transform the initial object as input for the newly created ValueSupplemented Pair</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewValue,
			ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue,
			Action<TS, TR> onValue)
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			IPipeOption pipeOption;
			return (i =>
			{
				IValueAndSupplement<TW, TR> toNewValueState;
				var pipeInput = (PipeBase<TV, TS>) source(i);

				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TR> pipeBaseWhenBreak, out pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					var newValue = toNewValue(pipeInput.SupplementVal);
					toNewValueState = newInitValueSupplementedValue(newValue);
					onValue?.Invoke(pipeInput.SupplementVal, toNewValueState.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TR>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked.</typeparam>
		/// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface.</typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <typeparam name="TS">The type that is the Supplemented value of the source parameter.</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue"></param>
		/// <param name="newValueAndSupplement"></param>
		/// <param name="onValue"></param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<PipeBase<TV, TS>, TW> toNewValue,
			Func<ToValueSupplementValue<TW, TW, TR>> newValueAndSupplement,
			Action<TS, TR> onValue)
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			IPipeOption pipeOption;
			TW newValue;
			return (i =>
			{
				IValueAndSupplement<TW, TR> toNewValueState;

				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TR> pipeBaseWhenBreak, out pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					newValue = toNewValue(pipeInput);
					toNewValueState = newValueAndSupplement()(newValue);
					onValue?.Invoke(pipeInput.SupplementVal, toNewValueState.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TR>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Transform: Converts one or both types of the IValueAndSupplement source interface.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="toNewValue">Functions that converts the initial supplemented val to a new val of another type that 
		/// serves as input parameter of the newInitValueSupplementedValue parameter.</param>
		/// <param name="newInitValueSupplementedValue">Func that returns a pipe where the input parameter takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
		/// <param name="onValue">Action that takes the initial supplemented  of the toNewValue parameter and the newly created Supplemented value of instance created by the newInitValueSupplementedValue parameter.
		/// The action can be used to transform the initial object as input for the newly created ValueSupplemented Pair</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TW, TR> Then<TI, TV, TW, TR, TS>(
			this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TW> toNewValue, Func<ToValueSupplementValue<TW, TW, TR>> newInitValueSupplementedValue,
			Action<TS, TR> onValue)
			where TV : new()
			where TW : new()
			where TR : new()
			where TS : new()
		{
			IPipeOption pipeOption;
			return (i =>
			{
				IValueAndSupplement<TW, TR> toNewValueState;

				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry(pipeInput, out PipeBase<TW, TR> pipeBaseWhenBreak, out pipeOption))
				{
					return pipeBaseWhenBreak;
				}

				try
				{
					if (pipeOption.ConditionIsMet == false)
						pipeOption = new PipeOption(false, ((IPipeOption) pipeInput).Extensions);

					var newValue = toNewValue(pipeInput.SupplementVal);
					toNewValueState = newInitValueSupplementedValue()(newValue);
					onValue?.Invoke(pipeInput.SupplementVal, toNewValueState.SupplementVal);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TW, TR>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
			});
		}

		#endregion

		#region Then

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Action delegate that takes as parameters the val and the supplemented value of the source parameter</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Action<TV, TS> apply)
			where TV : new()
			where TS : new()
		{
			return (i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TS, TV>(pipeInput, out PipeBase<TS, TV> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(inputToPipeBase.SupplementVal, inputToPipeBase.Val);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TS, TV> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TS, TV>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Action delegate that takes as parameters the supplemented value and val value of the source parameter</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Action<TS, TV> apply)
			where TV : new()
			where TS : new()
		{
			return (i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(inputToPipeBase.SupplementVal, inputToPipeBase.Val);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Action delegate that takes as parameters the val and the supplemented value of the source parameter</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Action<IValueAndSupplement<TV, TS>> apply)
			where TV : new()
			where TS : new()
		{
			return (i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(inputToPipeBase);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			});
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Func delegate that takes as parameter the supplemented and val values of the source and returns an object of the val type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<IValueAndSupplement<TS, TV>, TV> apply)
			where TV : new()
			where TS : new()
		{
			TV ret = default(TV);
			return (i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				var valAndSupplementVal = new ValueAndSupplement<TS, TV>(inputToPipeBase.SupplementVal, inputToPipeBase.Val);

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						ret = apply(valAndSupplementVal);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(ret, valAndSupplementVal.Val, pipeOption);
			});
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Func delegate that takes as parameter the supplemented snd val value of the source and returns an object of the Supplemented value type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<IValueAndSupplement<TS, TV>, TS> apply)
			where TV : new()
			where TS : new()
		{
			return (i =>
			{
				TS ret = default(TS);
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out IPipeOption pipeOption))
				{
					return inputToPipeBase;
				}

				var valAndSupplementVal = new ValueAndSupplement<TS, TV>(inputToPipeBase.SupplementVal, inputToPipeBase.Val);

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						ret = apply(valAndSupplementVal);
						if (!pipeInput.ValidateIsValid(new Some<TV, TS>(new ValueAndSupplement<TV, TS>(valAndSupplementVal.SupplementVal, ret), pipeOption), out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(ex, pipeOption);
				}

				return pipeInput.WrapPipeLineResult(valAndSupplementVal.SupplementVal, ret, pipeOption);
			});
		}

		#endregion

		#region from v to s

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Func delegate that takes as parameter the Supplemented value of the source and returns an object of the Val type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TV> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				TV ret = default(TV);
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TS, TV>(pipeInput, out PipeBase<TS, TV> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						ret = apply(inputToPipeBase.Val);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TS, TV> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TS, TV>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, ret, pipeOption);
			};
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Func delegate that takes as parameter the Val value of the source and returns an object of the Supplemented value type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TS> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				TS ret = default(TS);
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						ret = apply(inputToPipeBase.Val);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, ret, pipeOption);
			};
		}

		#endregion

		#region on S returning s

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// Type inference warning! Func<TS, TS> call can be ambigous with Action<TS>.Use typed lambda notation Example: (Car c) => CarExt.DriveFast(c)
		/// </summary>
		/// <typeparam name="TI"> The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV"> The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS"> The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source"> The pipe source object that implements the IValueAndSupplement interface.</param>
		/// <param name="apply"> Func delegate that takes as parameter the Supplemented value of the source and returns an object of the Supplemnted value type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, TS> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(inputToPipeBase.SupplementVal);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Action delegate that takes the Supplemented Value of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Action<TS> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(inputToPipeBase.SupplementVal);
						if (!pipeInput.ValidateIsValid(inputToPipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		#endregion

		#region on V returning V

		/// <summary>
		/// Type inference warning! Func<TV, TV> call can be ambigous with Action<TV>. Use typed lambda notation Example: (Car c) => CarExt.DriveFast(c)
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Func delegate that takes the Val value of the source and returns an object of the Val type of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, TV> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				TV ret;
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						ret = apply(inputToPipeBase.Val);
					}
					else
					{
						ret = inputToPipeBase.Val;
					}

					if (!pipeInput.ValidateIsValid(new Some<TV, TS>(new ValueAndSupplement<TV, TS>(ret, inputToPipeBase.SupplementVal), pipeOption), out PipeBase<TV, TS> invalidOption))
						return invalidOption;
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(ret, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Then: defines the action or function to perform in a pipeline segment when invoked.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="apply">Action delegate that takes the Val value of the source.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Action<TV> apply)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> pipeBase, out var pipeOption))
				{
					return pipeBase;
				}

				try
				{
					if (!pipeOption.ConditionIsMet.HasValue || pipeOption.ConditionIsMet.Value)
					{
						apply(pipeBase.Val);
						if (!pipeInput.ValidateIsValid(pipeBase, out PipeBase<TV, TS> invalidOption))
							return invalidOption;
					}
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(pipeBase.Val, pipeBase.SupplementVal, pipeOption);
			};
		}

		#endregion

		#region iff

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="predicate">Takes an expression with input parameter the out object of the source, and returns a boolean.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<IValueAndSupplement<TV, TS>, bool> predicate)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (predicate(inputToPipeBase))
					{
						pipeOption = new PipeOption(true, pipeOption.Extensions);
						return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
					}

					pipeOption = new PipeOption(false, pipeOption.Extensions);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="predicate">Expression that takes a Val value of the out object of the source and returns a boolean.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TV, bool> predicate)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (predicate(inputToPipeBase.Val))
					{
						pipeOption = new PipeOption(true, pipeOption.Extensions);
						return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
					}

					pipeOption = new PipeOption(false, pipeOption.Extensions);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), pipeOption);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <param name="predicate">Expression that takes a Supplemented value of the out object of the source and returns a boolean.</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
			Func<TS, bool> predicate)
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> inputToPipeBase, out var pipeOption))
				{
					return inputToPipeBase;
				}

				try
				{
					if (predicate(inputToPipeBase.SupplementVal))
					{
						pipeOption = new PipeOption(true, pipeOption.Extensions);
						return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
					}

					pipeOption = new PipeOption(false, pipeOption.Extensions);
				}
				catch (Exception ex)
				{
					return pipeInput.CreateException<TV, TS>(new InvalidOperationException(ex.Message), null);
				}

				return pipeInput.WrapPipeLineResult(inputToPipeBase.Val, inputToPipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> Else<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
			where TI : new()
			where TV : new()
			where TS : new()
		{
			bool PredicateTrue(TS input) => true;
			return source.Iff(PredicateTrue);
		}

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// Use EndIffStrong if at least one branch must result in a successful predicate.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		public static ToValueSupplementValue<TI, TV, TS> EndIff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
			where TI : new()
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var pipeInput = (PipeBase<TV, TS>) source(i);
				if (!pipeInput.ContinuePipeLineEntry<TV, TS>(pipeInput, out PipeBase<TV, TS> pipeBase, out var pipeOption))
				{
					return pipeBase;
				}

				pipeOption = new PipeOption(null, pipeOption.Extensions);
				return pipeInput.WrapPipeLineResult(pipeBase.Val, pipeBase.SupplementVal, pipeOption);
			};
		}

		/// <summary>
		/// Iff ElseEndIff EndIffStrong: methods that enable branching of the pipe based on predicate.
		/// EndIffStrong: At least one branch must result in a successful predicate. If not, this will be treated as an exception.
		/// </summary>
		/// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
		/// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
		/// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
		/// <param name="source">The pipe source object of type ToValueSupplementValue</param>
		/// <returns>An object of type ToValueSupplementValue (Func delegate that returns an object that implements the IValueAndSupplement interface when invoked with an object of type TI.</returns>
		/// <example>
		/// <code>
		/// var carInit = Pipe.Init<Car, Unit>(_ => new Unit())
		/// 	.Then(c => c.Mark = "Honda")
		/// 	.Iff(CarExt.IsHonda)
		/// 		.Then(CarExt.DriveFast)
		/// 	.Iff(CarExt.IsToyota)
		/// 		.Then(CarExt.DriveSlow)
		/// 	.EndIffStrong()
		/// </code>
		/// </example>
		public static ToValueSupplementValue<TI, TV, TS> EndIffStrong<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
			where TI : new()
			where TV : new()
			where TS : new()
		{
			return i =>
			{
				var inputToPipeBase = (PipeBase<TV, TS>) source(i);
				if (!inputToPipeBase.ContinuePipeLineEntry<TV, TS>(
					inputToPipeBase,
					out PipeBase<TV, TS> option,
					out var pipeOption))
				{
					return option;
				}

				if (pipeOption.ConditionIsMet == false)
				{
					return inputToPipeBase.CreateException<TV, TS>(new InvalidOperationException("EndIffStrong supposes at least one Iff returns truely. Add an Iff with a truely evaluated predicate"), pipeOption);
				}

				return inputToPipeBase.WrapPipeLineResult(option.Val, option.SupplementVal, option);
			};
		}

		#endregion
	}
}