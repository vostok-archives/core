namespace Vostok.Flow
{
    public interface IDecoder
    {
        string Encode(string str);
        string Decode(string str);
    }
}