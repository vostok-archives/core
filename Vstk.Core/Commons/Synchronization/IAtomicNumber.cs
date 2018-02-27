namespace Vstk.Commons.Synchronization
{
    public interface IAtomicNumber<T>
    {
        T Value { get; set; }
        T Increment();
        T Decrement();
        T Add(T diff);
        bool TryIncreaseTo(T newValue);
    }
}