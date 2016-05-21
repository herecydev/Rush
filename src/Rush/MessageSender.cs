using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Rush 
{
	internal class MessageSender<T> : ISendingStream<T>
	{
		private readonly IEnumerable<ISendingChannel<T>> _channels;
		private readonly ILogger<MessageSender<T>> _logger;

		public MessageSender(IEnumerable<ISendingChannel<T>> channels, ILogger<MessageSender<T>> logger)
		{
			_channels = channels;
			_logger = logger;
		}

		public Task SendAsync(T message) => SendAsync(message, CancellationToken.None);

		public Task SendAsync(T message, CancellationToken cancellationToken)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			_logger.LogInformation($"Sending message of type {typeof(T)}.");
			return Send(message, cancellationToken);
		}

		private async Task Send(T message, CancellationToken cancellationToken)
		{
			var operationalChannel = _channels.FirstOrDefault(stream => stream.Operational);

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