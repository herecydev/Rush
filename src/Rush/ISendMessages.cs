using System.Threading.Tasks;

namespace Rush
{
    public interface ISendMessages
    {
		Task SendAsync<T>(T message);
		Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request);
    }
}
