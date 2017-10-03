using Vostok.Flow;

namespace Vostok.Commons.Model
{
    public static class RequestPriorityContext
    {
        private const string ContextKey = "Vostok.RequestPriority";

        static RequestPriorityContext()
        {
            Context.Configuration.DistributedProperties.Add(ContextKey);
        }

        public static RequestPriority? Current
        {
            get
            {
                var value = Context.Properties.Get(ContextKey, int.MinValue);
                if (value < 0)
                    return null;

                return (RequestPriority) value;
            }
            set
            {
                if (value == null)
                {
                    Context.Properties.RemoveProperty(ContextKey);
                }
                else
                {
                    Context.Properties.Set(ContextKey, (int) value.Value);
                }
            }
        }
    }
}