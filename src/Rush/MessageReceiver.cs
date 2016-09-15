using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Rush
{
	internal class MessageReceiver<T> : IReceivingStream<T>
	{
		private readonly IObservable<T> _channels;
		private readonly ILogger<MessageReceiver<T>> _logger;

		public MessageReceiver(IEnumerable<IReceivingChannel<T>> channels, ILogger<MessageReceiver<T>> logger)
		{
			_channels = channels.Merge().Retry();
			_logger = logger;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (observer == null) throw new ArgumentNullException(nameof(observer));

			_logger.LogInformation($"Creating subscription for type {typeof(T)}.");

			return _channels.Subscribe(observer);
		}
	}
}
