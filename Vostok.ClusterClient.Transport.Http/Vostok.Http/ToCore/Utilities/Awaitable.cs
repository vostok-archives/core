using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities
{
    public struct Awaitable<T>
    {
        public Awaitable(Task<T> task)
        {
            this.task = task;
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return task.GetAwaiter();
        }

        public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext)
        {
            return task.ConfigureAwait(continueOnCapturedContext);
        }

        public void Wait()
        {
            task.Wait();
        }

        public T Result => task.Result;

        public bool IsCompleted => task.IsCompleted;

        public static implicit operator Task<T>(Awaitable<T> source)
        {
            return source.task;
        }

        private readonly Task<T> task;
    }
}