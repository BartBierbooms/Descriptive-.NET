using System.Collections.Generic;

namespace Piping
{
    public interface IPipeOption
    {
        bool? ConditionIsMet { get; }
        IList<IValueAndSupplementExtension> Extensions { get; }
    }

    internal class PipeOption : IPipeOption
    {
        internal readonly bool? ConditionMet;
        private readonly IList<IValueAndSupplementExtension> definedExtension;

        public bool? ConditionIsMet => ConditionMet;

        public IList<IValueAndSupplementExtension> Extensions => definedExtension;

        internal PipeOption(bool? conditionIsMet, IList<IValueAndSupplementExtension> extensions)
        {
            ConditionMet = conditionIsMet;
            definedExtension = extensions;
        }
    }
}
