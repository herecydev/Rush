using System.Threading;
using System.Threading.Tasks;

namespace Rush
{
	public interface IMessageStream
	{
		Task SendAsync<T>(T message);
		Task SendAsync<T>(T message, CancellationToken cancellationToken);
	}
}
