using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rush
{
	public class MessageSender<T> : ISendMessages<T>
	{
		private readonly IEnumerable<IMessageStream> _messageStreams;

		public MessageSender(IEnumerable<IMessageStream> messageStreams)
		{
			_messageStreams = messageStreams;
		}

		public async Task SendAsync(T message)
		{
			var operationalStream = _messageStreams.FirstOrDefault(stream => stream.Operational);

			if (operationalStream == null)
				throw new InvalidOperationException("There are no operational message streams.");

			try
			{
				await operationalStream.SendAsync(message);
			}
			catch (Exception)
			{
				await SendAsync(message);
			}
		}

		public Task<TResponse> SendAsync<TResponse>(T request)
		{
			throw new NotImplementedException();
		}
	}
}
