using System.Collections.Generic;

namespace Piping
{
    public interface IPipeOption
    {
        bool? ConditionIsMet { get; }
        IList<IValueAndSupplementExtension> Extensions { get; }
    }

    public class PipeOption : IPipeOption
    {
        internal readonly bool? ConditionMet;
        private readonly IList<IValueAndSupplementExtension> definedExtension;

        public bool? ConditionIsMet => ConditionMet;

        public IList<IValueAndSupplementExtension> Extensions => definedExtension;

        public PipeOption(bool? conditionIsMet, IList<IValueAndSupplementExtension> extensions)
        {
            ConditionMet = conditionIsMet;
            definedExtension = extensions;
        }
        public static PipeOption PipeOptionNone {get{return new PipeOption(null, null);}} 
    }
}
