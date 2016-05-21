using System.Threading;
using System.Threading.Tasks;

namespace Rush
{
	public interface IMessageStream<T>
	{
		Task SendAsync(T message);
		Task SendAsync(T message, CancellationToken cancellationToken);
	}
}
