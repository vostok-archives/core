﻿using System;
using System.Linq;
using System.Threading.Tasks;
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

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            clients[ThreadSafeRandom.Next(clients.Length)].Push(routingKey, item, timestamp);
        }

        public Task FlushAsync()
        {
            return Task.WhenAll(clients.Select(client => client.FlushAsync()));
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