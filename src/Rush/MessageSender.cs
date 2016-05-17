using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Rush 
{
	internal class MessageSender : IMessageStream
	{
		private readonly IMappingDictionary _mappingProvider;
		private readonly ILogger<MessageSender> _logger;

		public MessageSender(IMappingDictionary mappingProvider, ILogger<MessageSender> logger)
		{
			_mappingProvider = mappingProvider;
			_logger = logger;
		}

		public Task SendAsync<T>(T message) => SendAsync(message, CancellationToken.None);

		public Task SendAsync<T>(T message, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Sending message of type {typeof(T)}.");
			return Send(message, cancellationToken);
		}

		private async Task Send<T>(T message, CancellationToken cancellationToken)
		{
			var channels = _mappingProvider.GetSendingChannels<T>();
			var operationalChannel = channels.FirstOrDefault(stream => stream.Operational);

			if (operationalChannel == null)
				throw new InvalidOperationException("There are no operational message channels.");

			try
			{
				await operationalChannel.SendAsync(message, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"Operational channel faulted when sending message of type {typeof(T)}.", ex);
				await Send(message, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}