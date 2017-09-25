using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Flow;

namespace Vostok.Clusterclient.Transport
{
    // CR(iloktionov): 1. Почему эта штука не задействуется по умолчанию? Ее, конечно же, будут забывать добавлять руками.
    //                    Заведи флажок в конфигурации (вставлять ли в запросы контекстные свойства) с дефолтным значением true.
    //                    В конструкторе ClusterClient'а же можно уже автоматически заворачивать транспорт в этот декоратор, коли флажок выставлен.
    // CR(iloktionov): 2. С учетом (1) нет хороших причин выставлять этот класс в public.

    public class TransportWithDistributedContext : ITransport
    {
        private readonly ITransport transport;

        public TransportWithDistributedContext(ITransport transport)
        {
            this.transport = transport;
        }

        public Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var newRequest = BuildRequestWithDistributedContext(request);
            return transport.SendAsync(newRequest, timeout, cancellationToken);
        }

        private Request BuildRequestWithDistributedContext(Request request)
        {
            var distributedContext = Context.SerializeDistributedProperties();

            // CR(iloktionov): Двойное перечисление. Не забывай, что SerializeDistributedProperties() работает лениво.
            if (distributedContext == null || !distributedContext.Any())
            {
                return request;
            }

            // CR(iloktionov): Зачем ты вообще руками взялся делать массивы? Есть же request.WithHeader(), который решает нужную задачу в 1 строчку.
            // CR(iloktionov): Сразу наловил багов: что, если в запросе уже был одноименный заголовок? Будут дубликаты в массиве, из-за которых перестанет выполняться один из главных инвариантов класса Headers (он как словарик должен работать).
            // CR(iloktionov): Эффективности тут явно только убавилось из-за конского ToArray() (посмотри, как эффективно внутри Headers добавляются новые заголовки, пока нет конфликтов). 
            var newHeaders = distributedContext.Select(x => new Header(Encode(MakeHadearKey(x.Key)), Encode(x.Value)));
            if (request.Headers != null)
            {
                newHeaders = newHeaders.Concat(request.Headers);
            }

            var headersArray = newHeaders.ToArray();
            return new Request(request.Method, request.Url, request.Content, new Headers(headersArray, headersArray.Length));
        }

        // CR(iloktionov): Опечатка.
        private static string MakeHadearKey(string key)
        {
            // CR(iloktionov): Заголовки так не именуются. Во-первых, с большой буквы. Во-вторых, кастомные (не по RFC) всегда начинаются с "X-". В третьих, давай константу в HeaderNames.
            return "distributed_context/" + key;
        }

        private static string Encode(string str)
        {
            // CR(iloktionov): Неэффективно (новые строки будут выделяться даже когда ничего энкодить и не надо было), по крайней мере под фреймворком.
            // CR(iloktionov): Заюзай местный UrlEncodingHelper (он вручную соптимизирован на этот счет).
            return Uri.EscapeUriString(str);
        }
    }
}
