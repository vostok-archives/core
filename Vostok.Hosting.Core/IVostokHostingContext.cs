namespace Vostok.Hosting
{
    public interface IVostokHostingContext
    {
        IVostokHostingEnvironment Current { get; set; }
    }
}