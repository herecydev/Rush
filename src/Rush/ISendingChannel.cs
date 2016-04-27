using System.Threading.Tasks;

namespace Rush
{
	public interface ISendingChannel
	{
		bool Operational { get; }
		Task SendAsync<T>(T message);
	}
}
