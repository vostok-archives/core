using System;
using System.Linq;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Airlock
{
    public class ParallelAirlockClient : IAirlockClient, IDisposable
    {
        private readonly AirlockClient[] clients;

        public ParallelAirlockClient(AirlockConfig config, int parallelism, ILog log = null)
        {
            clients = new AirlockClient[parallelism];

            for (var i = 0; i < parallelism; i++)
            {
                clients[i] = new AirlockClient(config, log);
            }
        }

        public long LostItemsCount => clients.Sum(client => client.LostItemsCount);

        public long SentItemsCount => clients.Sum(client => client.SentItemsCount);

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            clients[ThreadSafeRandom.Next(clients.Length)].Push(routingKey, item, timestamp);
        }

        public void Dispose()
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }
    }
}
