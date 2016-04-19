using System.Threading.Tasks;

namespace Rush
{
	public interface IMessageStream
	{
		Task SendAsync<T>(T message);
	}
}
