using System.Threading.Tasks;

namespace Rush
{
    public interface ISendMessages<T>
    {
		Task SendAsync(T message);
		Task<TResponse> SendAsync<TResponse>(T request);
    }
}
