using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rush
{
	public class MessageSender : ISendMessages
	{
		private readonly IEnumerable<IMessageStream> _messageStreams;
		private readonly IOptions<MessageSenderOptions> _messageSenderOptions;

		public MessageSender(IEnumerable<IMessageStream> messageStreams, IOptions<MessageSenderOptions> messageSenderOptions)
		{
			_messageStreams = messageStreams;
			_messageSenderOptions = messageSenderOptions;
		}

		public Task SendAsync<T>(T message)
		{
			if (_messageSenderOptions.Value.StreamSelector == StreamSelector.Single)
				return _messageStreams.First(stream => stream.Operational).SendAsync(message);

			var sendTasks = new List<Task>();

			foreach (var stream in _messageStreams.Where(stream => stream.Operational))
				sendTasks.Add(stream.SendAsync(message));

			return Task.WhenAll(sendTasks);
		}

		public Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
