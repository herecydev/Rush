using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Rush
{
	internal class MessageReceiver<T> : IReceivingStream<T>
	{
		private readonly IEnumerable<IReceivingChannel<T>> _channels;
		private readonly ILogger<MessageReceiver<T>> _logger;

		public MessageReceiver(IEnumerable<IReceivingChannel<T>> channels, ILogger<MessageReceiver<T>> logger)
		{
			_channels = channels;
			_logger = logger;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			_logger.LogInformation($"Creating subscription for type {typeof(T)}.");

			var subscription = new Subscription();

			foreach (var channel in _channels)
				subscription.AddDisposable(channel.Subscribe(observer));

			return subscription;
		}

		private sealed class Subscription : IDisposable
		{
			private ICollection<IDisposable> _disposables = new List<IDisposable>();
			private bool _disposed;

			public void AddDisposable(IDisposable disposable) => _disposables.Add(disposable);

			public void Dispose()
			{
				lock (_disposables)
				{
					if (!_disposed)
					{
						foreach (var disposable in _disposables)
							disposable?.Dispose();

						_disposed = true;
					}
				}
			}
		}
	}
}
