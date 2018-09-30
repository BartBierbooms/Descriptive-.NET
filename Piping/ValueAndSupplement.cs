namespace Piping
{
    public class ValueAndSupplement<TV, TS> : IValueAndSupplement<TV, TS>
    {
        private readonly TS supplementPart;
        private readonly TV value;

        public ValueAndSupplement(TV val, TS supplement)
        {
            value = val;
            supplementPart = supplement;
        }

        public (TV, TS) Pair => (value, supplementPart);
        public TV Val => value;

        public TS SupplementVal => supplementPart;


    }
}
