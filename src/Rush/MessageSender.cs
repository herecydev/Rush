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
			{
				var operationalStream = _messageStreams.FirstOrDefault(stream => stream.Operational);

				if (operationalStream == null)
					throw new InvalidOperationException("There are no operational message streams.");

				return operationalStream.SendAsync(message);
			}
			else
			{
				var operationalStreams = _messageStreams.Where(stream => stream.Operational);
				if (!operationalStreams.Any())
					throw new InvalidOperationException("There are no operational message streams.");

				var sendTasks = new List<Task>();

				foreach (var stream in operationalStreams)
					sendTasks.Add(stream.SendAsync(message));

				return Task.WhenAll(sendTasks);
			}
		}

		public Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
