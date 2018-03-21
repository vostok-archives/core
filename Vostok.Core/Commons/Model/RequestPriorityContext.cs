using Vstk.Flow;

namespace Vstk.Commons.Model
{
    public static class RequestPriorityContext
    {
        private const string contextKey = "Vostok.RequestPriority";

        static RequestPriorityContext()
        {
            Context.Configuration.DistributedProperties.Add(contextKey);
        }

        public static RequestPriority? Current
        {
            get
            {
                var value = Context.Properties.Get(contextKey, int.MinValue);
                if (value < 0)
                    return null;

                return (RequestPriority) value;
            }
            set
            {
                if (value == null)
                {
                    Context.Properties.RemoveProperty(contextKey);
                }
                else
                {
                    Context.Properties.Set(contextKey, (int) value.Value);
                }
            }
        }
    }
}