using System;
using System.Collections.Generic;
using System.Text;

namespace Piping
{
    public enum ValueAndSupplementType
    {
        None = 0,
        Some = 1,
        Exception = 2,
    }

    public interface IValueAndSupplementType<T> where T : struct, IConvertible
    {
        int PipeType { get; }
    }

    public static class ValueAndSupplementTypeEx {

        public static ValueAndSupplementType ToValueAndSupplementType<T>(this T source) where T : struct, IConvertible {
            var iEnum = (int)(IConvertible)source;
            switch (iEnum)
            {
                case 0:
                    return ValueAndSupplementType.None;
                case 1:
                    return ValueAndSupplementType.Some;
                case 2:
                    return ValueAndSupplementType.Exception;
                default:
                    return ValueAndSupplementType.None;
            }
        }
    }
}
