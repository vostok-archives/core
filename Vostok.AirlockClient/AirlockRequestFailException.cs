namespace Vostok.AirlockClient
{
    public class AirlockRequestFailException : AirlockException
    {
        public AirlockResponse Response { get; }

        public AirlockRequestFailException(AirlockResponse response) => Response = response;
    }
}