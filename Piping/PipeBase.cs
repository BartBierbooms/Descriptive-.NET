using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Piping
{
    public abstract class PipeBase<TV, TS> : IValueAndSupplement<TV, TS>, IPipeOption
        where TV: new()
        where TS : new()
    {

        public abstract bool TestInputIsInvariant<TI>(
            TI input);

		public abstract PipeBase<TA, TB> CreateException<TA, TB>(
            Exception ex, 
            IPipeOption pipeOption)
            where TA : new()
            where TB : new();

		public abstract PipeBase<TV, TS> CreateNone(
            IPipeOption pipeOption);

        public abstract PipeBase<TV, TS> CreateSome(
            IValueAndSupplement<TV, TS> val,  
            IPipeOption pipeOption);

        public abstract PipeBase<TV, TS> CreateValidation(
            IValueAndSupplement<TV, TS> val,
            IPipeOption pipeOption, 
            ValidationResult result);

        public abstract TV Val { get; }
        public abstract TS SupplementVal { get; }

		protected abstract bool? ConditionIsMet{ get; }

		bool? IPipeOption.ConditionIsMet => ConditionIsMet;

		protected abstract IList<IValueAndSupplementExtension> PipebaseExtensions { get; }

		IList<IValueAndSupplementExtension> IPipeOption.Extensions => PipebaseExtensions;

		public abstract bool ContinuePipeLineEntry<TA, TB, TC, TD>(
            IValueAndSupplement<TA, TB> pipeInput,
            out PipeBase<TC, TD> wrapInputIntoPipeBaseWhenBreak,
            out IPipeOption pipeOption)
        where TA : new()
        where TB : new()
        where TC : new()
        where TD : new();

        public abstract bool ContinuePipeLineEntry<TA, TB>(
            IValueAndSupplement<TA, TB> pipeInput,
            out PipeBase<TA, TB> wrapInputIntoPipeBase,
            out IPipeOption pipeOption)
             where TA : new()
             where TB : new();

        public abstract bool ContinuePipeLineEntry<TB, TA>(
            IValueAndSupplement<TA, TB> pipeInput,
            out PipeBase<TB, TA> wrapInputIntoPipeBase,
            out IPipeOption pipeOption)
              where TA : new()
              where TB : new();

        public abstract PipeBase<TA, TB> WrapPipeLineResult<TA, TB>(
            TA value, 
            TB supplementedValue, 
            IPipeOption pipeOption)
            where TA : new()
            where TB : new();

        public abstract PipeBase<TI, TA> WrapPipeLineResult<TI, TA>(
            Func<TA> init, 
            TI value, 
            PipeOption pipeOption, 
            bool isInit = false)
            where TI : new()
            where TA : new();

        public abstract bool ValidateIsValid<TA, TB>(
            PipeBase<TA, TB> source,
            out PipeBase<TA, TB> pipeBaseValidated)
            where TA : new()
            where TB : new();


        protected void ExecuteExtensions(
            IPipeOption pipeOption, 
            object val)
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
    }
}
