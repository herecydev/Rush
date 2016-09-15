using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rush
{
	internal class BroadcastSender<T> : ISendingStream<T>
	{
		private readonly IEnumerable<ISendingChannel<T>> _channels;
		private readonly ILogger<BroadcastSender<T>> _logger;

		public BroadcastSender(IEnumerable<ISendingChannel<T>> channels, ILogger<BroadcastSender<T>> logger)
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

		private Task Send(T message, CancellationToken cancellationToken)
		{
			var sends = new List<Task<bool>>();

			foreach (var channel in _channels)
				sends.Add(SendToChannel(message, channel, cancellationToken));

			return Task.WhenAll(sends).ContinueWith(tasks =>
			{
				if (tasks.Result.All(sent => !sent))
					throw new InvalidOperationException("There are no operational message channels.");
			}, cancellationToken);
		}

		private async Task<bool> SendToChannel(T message, ISendingChannel<T> channel, CancellationToken cancellationToken)
		{
			if (!channel.Operational)
				return false;

			try
			{
				await channel.SendAsync(message, cancellationToken).ConfigureAwait(false);
				return true;
			}
			catch (Exception ex) when (ex.GetType() != typeof(OperationCanceledException))
			{
				_logger.LogWarning($"Operational channel faulted when sending message of type {typeof(T)}.", ex);
				return await SendToChannel(message, channel, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
