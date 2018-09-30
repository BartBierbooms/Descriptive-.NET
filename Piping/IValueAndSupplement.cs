namespace Piping
{
    public interface IValueAndSupplement<out TV, out TS> 
    {
        TV Val { get; }
        TS SupplementVal { get; }

    }
}
