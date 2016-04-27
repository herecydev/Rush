using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rush
{
	internal class MessageSender : IMessageStream
	{
		private readonly IProvideMappings _mappingDictionary;
		private readonly ILogger<MessageSender> _logger;

		public MessageSender(IProvideMappings mappingDictionary, ILogger<MessageSender> logger)
		{
			_mappingDictionary = mappingDictionary;
			_logger = logger;
		}

		public Task SendAsync<T>(T message)
		{
			_logger.LogInformation($"Sending message of type {typeof(T)}.");
			return Send(message);
		}

		private async Task Send<T>(T message)
		{
			var channels = _mappingDictionary.GetSendingChannels<T>();
			var operationalStream = channels.FirstOrDefault(stream => stream.Operational);

			if (operationalStream == null)
				throw new InvalidOperationException("There are no operational message streams.");

			try
			{
				await operationalStream.SendAsync(message);
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"Operational stream faulted sending message of type {typeof(T)}", ex);
				await Send(message);
			}
		}
	}
}