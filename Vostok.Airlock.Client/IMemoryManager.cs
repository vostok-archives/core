namespace Vostok.Airlock
{
    internal interface IMemoryManager
    {
        bool TryReserveBytes(int amount);

        bool TryCreateBuffer(out byte[] buffer);
    }
}