using System.Threading.Tasks;

namespace Rush
{
	public interface IMessageStream
	{
		bool Operational { get; }
		Task SendAsync<T>(T message);
		Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request);
	}
}
