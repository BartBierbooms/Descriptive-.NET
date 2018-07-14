using System;
using System.Collections.Generic;

namespace Piping
{
    /// <summary>
    /// Class with extension methods on a ToValueSupplementValue delegate.
    ///     The returned value are also ToValueSupplementValue delegates.
    ///     That makes piping possible: The output of one call is the input of the next call.
    /// The ToValueSupplementValue delegate is a Func with an input parameter that must be supplied when invoked.
    ///     When invoked, the returned type is an option.
    ///     Piping is constructing your definitions, which is separate from executing (invoking).
    /// </summary>
    public static class Pipe
    {
        public delegate IValueAndSupplement<TV, TS> ToValueSupplementValue<in TI, out TV, out TS>(TI i);

        #region init

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">Unit type (The type Unit has no implementation)</typeparam>
        /// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
        /// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        /// <example>var pipeline = Pipe.Init<Car, Unit>();</example>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(IList<IValueAndSupplementExtension> postProcessings)
            where TV : Unit
            where TI : new()
        {
            return i =>
            {
                Func<TV> CreateUnit() => () => (TV)new Unit();
                try
                {
                    if (i != null && i is IInvariant<TI> && !((IInvariant<TI>)i).IsValid)
                    {
                        return Option<TI, TV>.SomeValidation(new ValueAndSupplement<TI, TV>(i, CreateUnit()()), new PipeOption(null, null), ((IInvariant<TI>)i).ToValidationResult<TI>());
                    }
                }
                catch (Exception ex)
                {
                    return Option<TI, TV>.Exception(ex, new PipeOption(null, null));
                }
                return ToOption<TI, TV>(CreateUnit(), i, new PipeOption(null, postProcessings), true);
            };
        }

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>()
            where TV : Unit
            where TI : new()
        {
            return i =>
            {
                Func<TV> CreateUnit() => () => (TV)new Unit();
                try
                {
                    if (i != null && i is IInvariant<TI> && !((IInvariant<TI>)i).IsValid)
                    {
                        return Option<TI, TV>.SomeValidation(new ValueAndSupplement<TI, TV>(i, CreateUnit()()), new PipeOption(null, null), ((IInvariant<TI>)i).ToValidationResult<TI>());
                    }
                }
                catch (Exception ex)
                {
                    return Option<TI, TV>.Exception(ex, new PipeOption(null, null));
                }
                return ToOption<TI, TV>(CreateUnit(), i, new PipeOption(null, null), true);
            };
        }

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <param name="init">Func delegate that creates an instance of type TV</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TV> init)
            where TV : new()
            where TI : new()
        {
            return i =>
            {
                try
                {
                    if (i != null && i is IInvariant<TI> && !((IInvariant<TI>)i).IsValid)
                    {
                        return Option<TI, TV>.SomeValidation(new ValueAndSupplement<TI, TV>(i, default(TV)), new PipeOption(null, null), ((IInvariant<TI>)i).ToValidationResult<TI>());
                    }
                }
                catch (Exception ex)
                {
                    return Option<TI, TV>.Exception(ex, new PipeOption(null, null));
                }
                return ToOption<TI, TV>(init, i, new PipeOption(null, null), true);
            };
        }

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <param name="init">Func delegate that creates an instance of type TV</param>
        /// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
        /// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TV> init, IList<IValueAndSupplementExtension> postProcessings)
            where TV : new()
            where TI : new()
        {
            return i =>
            {
                try
                {
                    if (i != null && i is IInvariant<TI> && !((IInvariant<TI>)i).IsValid)
                    {
                        return Option<TI, TV>.SomeValidation(new ValueAndSupplement<TI, TV>(i, default(TV)), new PipeOption(null, null), ((IInvariant<TI>)i).ToValidationResult<TI>());
                    }
                }
                catch (Exception ex)
                {
                    return Option<TI, TV>.Exception(ex, new PipeOption(null, null));
                }

                return ToOption<TI, TV>(init, i, new PipeOption(null, postProcessings), true);
            };
        }

        /// <summary>
        /// Usage: Init the pipe by providing another pipe that has the same signature.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <param name="init">Func delegate that creates an instance of type ToValueSupplementValue<TI, TV></param>
        /// <returns></returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(ToValueSupplementValue<TI, TI, TV> init)
            where TV : new()
            where TI : new()
        {
            return i => init(i);
        }

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <param name="init">Func delegate that takes an instance of type TI and returns an instance of type TV</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TI, TV> init)
            where TV : new()
            where TI : new()
        {
            return Init(init, null);
        }

        /// <summary>
        /// The input of a pipeline is an object to work on. You can initiate the pipeline with an (supplemented) object that works with the passed input. 
        /// Example: The pipeline is initialized with a Validation object (its Supplement).
        ///          The input of the pipeline (its Val) is a Car. The suplemented Validation object can be used to validate a specific instance of the car.
        ///          Use Unit (a type with no implementation) if no supplemented functionality is needed.
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <param name="init">Func delegate that takes an instance of type TI and returns an instance of type TV</param>
        /// <param name="postProcessings">The list of instances of objects that implements an IValueAndSupplementExtension interface. 
        /// After a method in the pipeline is executed, the PostProcess method of this Interface is called on both values of the returned IValueAndSupplement instance.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TI, TV> init, IList<IValueAndSupplementExtension> postProcessings)
            where TV : new()
            where TI : new()
        {
            return i =>
            {
                var pipeOtion = new PipeOption(null, postProcessings);
                TV nwevalue = default(TV);
                try
                {
                    if (i != null && i is IInvariant<TI> && !((IInvariant<TI>)i).IsValid)
                    {
                        return Option<TI, TV>.SomeValidation(new ValueAndSupplement<TI, TV>(i, nwevalue), new PipeOption(null, null), ((IInvariant<TI>)i).ToValidationResult<TI>());
                    }

                    nwevalue = init(i);
                    if (i.Equals(nwevalue))
                        return Option<TI, TV>.Exception(new InvalidOperationException("The Init function must create a new instance of an object"), null);
                }
                catch (Exception ex)
                {
                    return Option<TI, TV>.Exception(ex, pipeOtion);
                }
                if (nwevalue == null)
                {
                    return Option<TI, TV>.None(pipeOtion);
                }
                return new Some<TI, TV>(new ValueAndSupplement<TI, TV>(i, nwevalue), pipeOtion);
            };
        }

        #endregion
        #region TransForm
        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type that is the Supplemented value of the source parameter</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Func delegate that takes the Supplemented value of the source and returns the Supplemented value of the returned IValueAndSupplement interface</param>
        /// <param name="defaultTW">Needed for type inference: You provide default([class of type TW])</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TW> TransForm<TI, TV, TW, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TS, TW> toNewValue)
            where TI : new()
            where TV : new()
            where TW : new()
            where TS : new()
        {
            return (i =>
            {
                TW newValue = default(TW);

                if (!EvaluateToNewOption(source, out Option<TV, TW> invokedOption, i, out var pipeOption, out var sourceOption))
                {
                    return invokedOption;
                }
                try
                {
                    newValue = toNewValue(sourceOption.SupplementVal);
                }
                catch (Exception ex)
                {
                    return Option<TV, TW>.Exception(ex, pipeOption);
                }
                return ToOption<TV, TW>(sourceOption.Val, newValue, pipeOption);
            });
        }

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the object that is returned as the Val of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type was the Supplemented value of the source parameter</typeparam>
        /// <typeparam name="defaultTW">Needed for type inference: You provide default([class of type TW])</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Func delegate that takes the Val value of the source and returns the Val value of the returned IValueAndSupplement interface</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TS> TransForm<TI, TV, TW, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TV, TW> toNewValue)
            where TI : new()
            where TV : new()
            where TW : new()
            where TS : new()
        {
            return (i =>
            {
                TW newValue = default(TW);

                if (!EvaluateToNewOption(source, out Option<TW, TS> invokedOption, i, out var pipeOption, out var sourceOption))
                {
                    return invokedOption;
                }
                try
                {
                    newValue = toNewValue(sourceOption.Val);
                }
                catch (Exception ex)
                {
                    return Option<TW, TS>.Exception(ex, pipeOption);
                }
                return ToOption<TW, TS>(newValue, sourceOption.SupplementVal, pipeOption);
            });
        }

        //public static ToValueSupplementValue<TI, TW, TS> TransForm<TI, TV, TW, TS>(this ToValueSupplementValue<TI, TV, TS> source,
        //    Func<TV, TW> toNewValue
        //    )
        //    where TI : new()
        //    where TV : new()
        //    where TW : new()
        //    where TS : new()
        //{
        //    return (i =>
        //    {
        //        TW newValue = default(TW);

        //        if (!EvaluateToNewOption(source, out Option<TW, TS> invokedOption, i, out var pipeOption, out var sourceOption))
        //        {
        //            return invokedOption;
        //        }
        //        try
        //        {
        //            newValue = toNewValue(sourceOption.Val);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Option<TW, TS>.Exception(ex, pipeOption);
        //        }
        //        return ToOption<TW, TS>(newValue, sourceOption.SupplementVal, pipeOption);
        //    });
        //}

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type that is the Supplemented value of the source parameter</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Func delegate that takes the Supplemented value of the source and returns the Val value of the returned IValueAndSupplement interface</param>
        /// <param name="toNewSupplemtedValue">Func delegate that takes the returned value of toNewValue as input and returns the supplemented value of the returned IValueAndSupplement interface</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
           Func<TS, TW> toNewValue,
           Func<TR> toNewSupplemtedValue)
            where TI : new()
            where TV : new()
            where TR : new()
            where TW : new()
            where TS : new()
        {
            var newConvertTo = Pipe.Init<TW, TR>(toNewSupplemtedValue);
            return source.Transform(toNewValue, newConvertTo, ((val, valAndExtension) => { }));
        }

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Func delegate that takes the Val value of the source and returns the Val value of the returned IValueAndSupplement interface</param>
        /// <param name="toNewSupplementedValue">Func delegate that takes the returned value of toNewValue as input and returns the Val value of the returned IValueAndSupplement interface</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
           Func<TV, TW> toNewValue, Func<TR> toNewSupplementedValue)
            where TI : new()
            where TV : new()
            where TR : new()
            where TW : new()
            where TS : new()
        {
            var newConvertTo = Pipe.Init<TW, TR>(toNewSupplementedValue);
            return source.Then(toNewValue, newConvertTo, ((val, valAndExtension) => { }));
        }

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Functions that converts the initial val to a new val of another type that 
        /// serves as input parameter of the newInitValueSupplementedValue parameter.</param>
        /// <param name="newInitValueSupplementedValue">parameter that takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
        /// <param name="onNewSupplementedValue">Action that takes the inital val of the toNewValue parameter and the newly created Supplemented value of instance created by the newInitValueSupplementedValue parameter.
        /// The action can be used to tranform the initial object as input for the newly created ValueSupplemented Pair</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
           Func<TV, TW> toNewValue,
           ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue,
           Action<TV, TR> onNewSupplementedValue)
            where TI : new()
            where TV : new()
            where TW : new()
            where TR : new()
            where TS : new()
        {
            return source.Then(toNewValue, newInitValueSupplementedValue, onNewSupplementedValue);
        }


        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue"></param>
        /// <param name="newInitValueSupplementedValue"></param>
        /// <returns></returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
           Func<TV, TW> toNewValue,
           ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue)
            where TI : new()
            where TV : new()
            where TW : new()
            where TR : new()
            where TS : new()
        {
            return source.Then(toNewValue, newInitValueSupplementedValue, (TV dummyTS, TR dummyTR) => { });
        }

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue"></param>
        /// <returns></returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
           Func<TS, TW> toNewValue,
           ToValueSupplementValue<TW, TW, TR> newInitValueSupplementedValue)
            where TI : new()
            where TV : new()
            where TW : new()
            where TR : new()
            where TS : new()
        {
            return source.Transform(toNewValue, newInitValueSupplementedValue, (TS dummyTS, TR dummyTR) => { });
        }

        /// <summary>
        /// TransForms: Converts one or both types of the IValueAndSupplement source interface. Example from source IValueAndSupplement<Car, Unit> to output IValueAndSupplement<Mail, BasketResult>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewSupplemtedValue">Func delegate that takes the returned value of toNewValue as input and returns the supplemented value of the returned IValueAndSupplement interface</param>
        /// <param name="onNewSupplementedValue">Action delegate that takes the original Supplemented object of the source and the Supplemented val of the object that is returned.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> TransForm<TI, TV, TW, TR, TS>(this ToValueSupplementValue<TI, TV, TS> source,
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
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Func delegate that takes the val of the source parameter and outputs a new type as val parameter of the object taht is returned</param>
        /// <param name="newValueAndSupplement">parameter that takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
        /// <param name="onValue">Action delegate that takes the original Supplemented object of the source and the Supplemented val of the object that is returned.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> Then<TI, TV, TW, TR, TS>(
            this ToValueSupplementValue<TI, TV, TS> source,
            Func<TV, TW> toNewValue, ToValueSupplementValue<TW, TW, TR> newValueAndSupplement,
            Action<TV, TR> onValue)
        {

            return (i =>
            {
                TW newValue = default(TW);
                IValueAndSupplement<TW, TR> toNewValueState;
                Option<TW, TR> option;
                Option<TV, TS> sourceOption;
                IPipeOption pipeOption;

                if (!EvaluateToNewOption(source, out option, i, out pipeOption, out sourceOption))
                {
                    return option;
                }
                try
                {
                    newValue = toNewValue(sourceOption.Val);
                    toNewValueState = newValueAndSupplement(newValue);
                    onValue?.Invoke(sourceOption.Val, toNewValueState.SupplementVal);
                }
                catch (Exception ex)
                {
                    return Option<TW, TR>.Exception(ex, pipeOption);
                }
                return ToOption<TW, TR>(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);

            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TW">The type of the object that is returned as the Val value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TR">The type of the object that is returned as the Supplemented value of the IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="toNewValue">Functions that converts the initial supplemented val to a new val of another type that 
        /// serves as input parameter of the newInitValueSupplementedValue parameter.</param>
        /// <param name="newInitValueSupplementedValue">parameter that takes the output from toNewValue to initiate an new ValueAndSupplement instance</param>
        /// <param name="onValue">Action that takes the inital supplemnted  of the toNewValue parameter and the newly created Supplemented value of instance created by the newInitValueSupplementedValue parameter.
        /// The action can be used to tranform the initial object as input for the newly created ValueSupplemented Pair</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TW, TR> Transform<TI, TV, TW, TR, TS>(
                this ToValueSupplementValue<TI, TV, TS> source,
                Func<TS, TW> toNewValue, ToValueSupplementValue<TW, TW, TR> newValueAndSupplement,
                Action<TS, TR> onValue)
        {
            return (i =>
            {
                TW newValue = default(TW);
                IValueAndSupplement<TW, TR> toNewValueState;
                Option<TW, TR> option;
                Option<TV, TS> sourceOption;
                IPipeOption pipeOption;

                if (!EvaluateToNewOption(source, out option, i, out pipeOption, out sourceOption))
                {
                    return option;
                }
                try
                {
                    newValue = toNewValue(sourceOption.SupplementVal);
                    toNewValueState = newValueAndSupplement(newValue);
                    onValue?.Invoke(sourceOption.SupplementVal, toNewValueState.SupplementVal);
                }
                catch (Exception ex)
                {
                    return Option<TW, TR>.Exception(ex, pipeOption);
                }
                return ToOption<TW, TR>(toNewValueState.Val, toNewValueState.SupplementVal, pipeOption);
            });
        }

        #endregion

        #region Then

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Action delegate that takes as parameters the val and the supplemented value of the source parameter</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source, 
            Action<TV, TS> apply)
        {
            return (i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TS, TV> option, out var pipeOption))
                {
                    return option;
                }

                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option.SupplementVal, option.Val);
                        if (!ValidateIsValid<TI, TS, TV>(option, out Option<TS, TV> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TS, TV>.Exception(ex, pipeOption);

                }
                return ToOption<TS, TV>(option.Val, option.SupplementVal, pipeOption);
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Action delegate that takes as parameters the supplemented value and the vakl of the source parameter</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<TS, TV> apply)
        {
            return (i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }

                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option.SupplementVal, option.Val);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(ex, pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply"></param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<IValueAndSupplement<TV, TS>> apply)
        {

            return (i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }

                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(ex, pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Func delegate that takes as parameter the source and returns an object of the val type of th source.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<IValueAndSupplement<TS, TV>, TV> apply)
        {

            return (i =>
            {
                TV ret = default(TV);

                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }

                var valAndSupplementVal = new ValueAndSupplement<TS, TV>(option.SupplementVal, option.Val);

                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        ret = apply(valAndSupplementVal);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(ex, pipeOption);
                }
                return ToOption<TV, TS>(ret, valAndSupplementVal.Val, pipeOption);
            });
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Func delegate that takes as parameter the source and returns an object of the Supplemted value type of the source.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<IValueAndSupplement<TS, TV>, TS> apply)
        {

            return (i =>
            {
                TS ret = default(TS);

                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out IPipeOption pipeOption))
                {
                    return option;
                }

                var valAndSupplementVal = new ValueAndSupplement<TS, TV>(option.SupplementVal, option.Val);

                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        ret = apply(valAndSupplementVal);
                        if (!ValidateIsValid<TI, TV, TS>(new Some<TV, TS>(new ValueAndSupplement<TV, TS>(valAndSupplementVal.SupplementVal, ret), pipeOption), out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(ex, pipeOption);

                }
                return ToOption<TV, TS>(valAndSupplementVal.SupplementVal, ret, pipeOption);
            });
        }
        #endregion

        #region from v to s

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Func delegate that takes as parameter the Supplemented val of the source and returns an object of the Val type of the source.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TS, TV> apply)
        {
            return i =>
            {
                TV ret = default(TV);

                if (!EvaluateToNewOption(source, i, out Option<TS, TV> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        ret = apply(option.Val);
                        if (!ValidateIsValid<TI, TS, TV>(option, out Option<TS, TV> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TS, TV>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TS, TV>(option.Val, ret, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Func delegate that takes as parameter the Val of the source and returns an object of the Supplemnted value type of the source.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TV, TS> apply)
        {
            return i =>
            {
                TS ret = default(TS);

                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        ret = apply(option.Val);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, ret, pipeOption);
            };
        }
        #endregion

        #region on S returning s

        ///// <summary>
        ///// Type inference warning! Func<TS, TS> call can be ambigous with Action<TS>. Use typed lambda notation Example: (Car c) => CarExt.DriveFast(c)
        ///// </summary>
        ///// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        ///// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        ///// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        ///// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        ///// <param name="apply">Func delegate that takes as parameter the Supplemented value of the source and returns an object of the Supplemnted value type of the source.</param>
        ///// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TS, TS> apply)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option.SupplementVal);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Action delegate that takes the Supplemented Value of the source</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<TS> apply)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option.SupplementVal);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }
        #endregion

        #region on V returning V
        ///// <summary>
        ///// Type inference warning! Func<TV, TV> call can be ambigous with Action<TV>. Use typed lambda notation Example: (Car c) => CarExt.DriveFast(c)
        ///// </summary>
        ///// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        ///// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        ///// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        ///// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        ///// <param name="apply">Func delegate that takes the Val of the source and returns an object of the Val type of the source</param>
        ///// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TV, TV> apply)
        {
            return i =>
            {
                TV ret = default(TV);

                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        ret = apply(option.Val);
                    }
                    else
                    {
                        ret = option.Val;
                    }
                    if (!ValidateIsValid<TI, TV, TS>(new Some<TV, TS>(new ValueAndSupplement<TV, TS>(ret, option.SupplementVal), pipeOption), out Option<TV, TS> invalidOption))
                        return invalidOption;

                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(ret, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="apply">Action delegate that takes the Val of the source.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<TV> apply)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption))
                {
                    return option;
                }
                try
                {
                    if (!option.ConditionMet.HasValue || option.ConditionMet.Value)
                    {
                        apply(option.Val);
                        if (!ValidateIsValid<TI, TV, TS>(option, out Option<TV, TS> invalidOption))
                            return invalidOption;
                    }
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }
        #endregion

        #region iff
        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="predicate">Takes an expression that inputs an IValueSupplemented interface type of the source and returns a boolean.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<IValueAndSupplement<TV, TS>, bool> predicate)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption, false))
                {
                    return option;
                }
                try
                {
                    if (predicate(option))
                    {
                        pipeOption = new PipeOption(true, pipeOption.Extensions);
                        return ToOption(option.Val, option.SupplementVal, pipeOption);
                    }
                    pipeOption = new PipeOption(false, pipeOption.Extensions);
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="predicate">Expression that takes a Val type of the source and returns a boolean.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TV, bool> predicate)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption, false))
                {
                    return option;
                }
                try
                {
                    if (predicate(option.Val))
                    {
                        pipeOption = new PipeOption(true, pipeOption.Extensions);
                        return ToOption(option.Val, option.SupplementVal, pipeOption);
                    }
                    pipeOption = new PipeOption(false, pipeOption.Extensions);
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <param name="predicate">Expression that takes a Supplemented type of the source and returns a boolean.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Iff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Func<TS, bool> predicate)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption, false))
                {
                    return option;
                }
                try
                {
                    if (predicate(option.SupplementVal))
                    {
                        pipeOption = new PipeOption(true, pipeOption.Extensions);
                        return ToOption(option.Val, option.SupplementVal, pipeOption);
                    }
                    pipeOption = new PipeOption(false, pipeOption.Extensions);
                }
                catch (Exception ex)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException(ex.Message), null);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> Else<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
        {
            bool PredicateTrue(TS input) => true;
            return source.Iff(PredicateTrue);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> EndIff<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption, false))
                {
                    return option;
                }
                pipeOption = new PipeOption(null, pipeOption.Extensions);
                return ToOption<TV, TS>(option.Val, option.SupplementVal, pipeOption);
            };
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TI">The type of the object that is passed into the pipeline delegate when invoked</typeparam>
        /// <typeparam name="TV">The type of the source object that is the Val of the source IValueAndSupplement interface</typeparam>
        /// <typeparam name="TS">The type of the source object that is the Supplemented value of the source IValueAndSupplement interface</typeparam>
        /// <param name="source">The pipe source object that implements the IValueAndSupplement interface.</param>
        /// <returns>Option, that implements the IValueAndSupplement interface</returns>
        public static ToValueSupplementValue<TI, TV, TS> EndIffStrong<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source)
        {
            return i =>
            {
                if (!EvaluateToNewOption(source, i, out Option<TV, TS> option, out var pipeOption, false))
                {
                    return option;
                }

                if (pipeOption.ConditionIsMet == false)
                {
                    return Option<TV, TS>.Exception(new InvalidOperationException("EndIffStrong supposes at least one Iff returns truely. Add an Iff with a truely evaluated predicate"), pipeOption);
                }
                return ToOption<TV, TS>(option.Val, option.SupplementVal, option);
            };
        }

        #endregion

        #region private function
        #region EvaluateToNewOption
        private static bool EvaluateToNewOption<TI, TV, TS, TW, TR>(ToValueSupplementValue<TI, TV, TS> source,
            out Option<TW, TR> sourceInvoked,
            TI input,
            out IPipeOption pipeOption,
            out Option<TV, TS> sourceOption,
            bool checkConditionMet = true)
        {
            var valueAndSupplementedVal = source(input);
            pipeOption = (IPipeOption)valueAndSupplementedVal;

            sourceOption = Option<TV, TS>.Some(new ValueAndSupplement<TV, TS>(valueAndSupplementedVal.Val, valueAndSupplementedVal.SupplementVal), pipeOption);

            if (checkConditionMet)
            {
                if (pipeOption.ConditionIsMet == false)
                {
                    pipeOption = new PipeOption(false, ((IPipeOption)valueAndSupplementedVal).Extensions);
                    if (!(valueAndSupplementedVal.GetType() == typeof(SomeException<TV, TS>)))
                    {
                        sourceInvoked = Option<TW, TR>.Some(new ValueAndSupplement<TW, TR>(default(TW), default(TR)), pipeOption);
                        return true;
                    }
                }
            }

            if (valueAndSupplementedVal.GetType() == typeof(Option<TV, TS>))
            {
                sourceInvoked = Option<TW, TR>.None(pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(SomeException<TV, TS>))
            {
                sourceInvoked = Option<TW, TR>.Exception(((SomeException<TV, TS>)valueAndSupplementedVal).ExceptionVal, pipeOption);
                return false;
            }

            if (valueAndSupplementedVal.GetType() == typeof(Validation<TV, TS>))
            {
                sourceInvoked = Validation<TW, TR>.SomeValidation(new ValueAndSupplement<TW, TR>(default(TW), default(TR)), pipeOption, ((Validation<TW, TR>)valueAndSupplementedVal).ValidationRet);
                return false;
            }

            //can be ignored
            sourceInvoked = Option<TW, TR>.Some(new ValueAndSupplement<TW, TR>(default(TW), default(TR)), pipeOption);
            return true;
        }

        private static bool EvaluateToNewOption<TI, TV, TS>(ToValueSupplementValue<TI, TV, TS> source,
            TI input, out Option<TV, TS> sourceInvoked, out IPipeOption pipeOption, bool checkConditionMet = true)
        {
            var valueAndSupplementedVal = source(input);
            pipeOption = (IPipeOption)valueAndSupplementedVal;

            if (checkConditionMet)
            {
                if (pipeOption.ConditionIsMet == false)
                {
                    pipeOption = new PipeOption(false, ((IPipeOption)valueAndSupplementedVal).Extensions);
                    if (!(valueAndSupplementedVal.GetType() == typeof(SomeException<TV, TS>)))
                    {
                        sourceInvoked = Option<TV, TS>.Some(new ValueAndSupplement<TV, TS>(valueAndSupplementedVal.Val, valueAndSupplementedVal.SupplementVal), pipeOption);
                        return true;
                    }
                }
            }

            if (valueAndSupplementedVal.GetType() == typeof(Option<TV, TS>))
            {
                sourceInvoked = Option<TV, TS>.None(pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(SomeException<TV, TS>))
            {
                sourceInvoked = Option<TV, TS>.Exception(((SomeException<TV, TS>)valueAndSupplementedVal).ExceptionVal, pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(Validation<TV, TS>))
            {
                sourceInvoked = Validation<TV, TS>.SomeValidation(new ValueAndSupplement<TV, TS>(valueAndSupplementedVal.Val, valueAndSupplementedVal.SupplementVal), pipeOption, ((Validation<TV, TS>)valueAndSupplementedVal).ValidationRet);
                return false;
            }

            sourceInvoked = Option<TV, TS>.Some(new ValueAndSupplement<TV, TS>(valueAndSupplementedVal.Val, valueAndSupplementedVal.SupplementVal), pipeOption);
            return true;
        }

        private static bool EvaluateToNewOption<TI, TS, TV>(ToValueSupplementValue<TI, TV, TS> source,
            TI input, out Option<TS, TV> sourceInvoked, out IPipeOption pipeOption, bool checkConditionMet = true)
        {
            var valueAndSupplementedVal = source(input);
            pipeOption = (IPipeOption)valueAndSupplementedVal;

            if (pipeOption.ConditionIsMet == false)
            {
                sourceInvoked = Option<TS, TV>.Some(new ValueAndSupplement<TS, TV>(valueAndSupplementedVal.SupplementVal, valueAndSupplementedVal.Val), pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(Option<TV, TS>))
            {
                sourceInvoked = Option<TS, TV>.None(pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(SomeException<TV, TS>))
            {
                sourceInvoked = Option<TS, TV>.Exception(((SomeException<TV, TS>)valueAndSupplementedVal).ExceptionVal, pipeOption);
                return false;
            }
            if (valueAndSupplementedVal.GetType() == typeof(Validation<TV, TS>))
            {
                sourceInvoked = Validation<TS, TV>.SomeValidation(new ValueAndSupplement<TS, TV>(valueAndSupplementedVal.SupplementVal, valueAndSupplementedVal.Val), pipeOption, ((Validation<TS, TV>)valueAndSupplementedVal).ValidationRet);
                return false;
            }
            sourceInvoked = Option<TS, TV>.Some(new ValueAndSupplement<TS, TV>(valueAndSupplementedVal.SupplementVal, valueAndSupplementedVal.Val), pipeOption);
            return true;
        }
        #endregion

        #region ToOption
        private static Option<TV, TS> ToOption<TV, TS>(TV value, TS supplementedValue, IPipeOption pipeOption)
        {

            if (value == null && supplementedValue == null)
            {
                return Option<TV, TS>.Exception(new InvalidOperationException("Null values for value and supplemented value are not allowed!"), pipeOption);
            }
            if (supplementedValue == null)
            {
                return Option<TV, TS>.Exception(new InvalidOperationException("Null values for supplemented value is not allowed!"), pipeOption);
            }

            try
            {
                ExecuteExtensions(pipeOption, value);
                ExecuteExtensions(pipeOption, supplementedValue);
            }
            catch (Exception ex)
            {
                return Option<TV, TS>.Exception(ex, pipeOption);
            }
            return new Some<TV, TS>(new ValueAndSupplement<TV, TS>(value, supplementedValue), pipeOption);

        }

        private static Option<TI, TV> ToOption<TI, TV>(Func<TV> init, TI value, PipeOption pipeOption, bool isInit = false)
        {

            TV nwevalue = default(TV);

            try
            {
                nwevalue = init();
                if (isInit && value.Equals(nwevalue))
                {
                    return Option<TI, TV>.Exception(new InvalidOperationException("The Init function must create a new instance of an object"), pipeOption);
                }
                ExecuteExtensions(pipeOption, value);
                ExecuteExtensions(pipeOption, nwevalue);
            }
            catch (Exception ex)
            {
                return Option<TI, TV>.Exception(ex, pipeOption);
            }
            if (nwevalue == null)
            {
                return Option<TI, TV>.None(pipeOption);
            }
            return new Some<TI, TV>(new ValueAndSupplement<TI, TV>(value, nwevalue), pipeOption);
        }

        #endregion

        #region Check IsValid
        private static bool ValidateIsValid<TI, TV, TS>(Option<TV, TS> source, out Option<TV, TS> validationOption)
        {
            validationOption = source;
            if (source.GetOptionType == OptionType.Some)
            {
                if (source.Val != null && source.Val is IInvariant<TV> && !((IInvariant<TV>)source.Val).IsValid)
                {
                    validationOption = Option<TV, TS>.SomeValidation(new ValueAndSupplement<TV, TS>(source.Val, source.SupplementVal), new PipeOption(null, null), ((IInvariant<TV>)source.Val).ToValidationResult());
                    return false;
                }
                if (source.SupplementVal != null && source.SupplementVal is IInvariant<TS> && !((IInvariant<TS>)source.SupplementVal).IsValid)
                {
                    validationOption = Option<TV, TS>.SomeValidation(new ValueAndSupplement<TV, TS>(source.Val, source.SupplementVal), new PipeOption(null, null), ((IInvariant<TS>)source.SupplementVal).ToValidationResult());
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Execute extensions
        private static void ExecuteExtensions(IPipeOption pipeOption, object val)
        {

            if (val == null)
                return;

            if (pipeOption?.Extensions != null)
            {
                foreach (var ext in pipeOption.Extensions)
                {
                    if (val is IExpose)
                    {
                        ext.PostProcess(val as IExpose);
                    }
                };
            }
        }
        #endregion
        #endregion
    }
}