using System.Threading;
using System.Threading.Tasks;

namespace Rush
{
	public interface ISendingChannel<T>
	{
		bool Operational { get; }
		Task SendAsync(T message, CancellationToken cancellationToken);
	}
}
