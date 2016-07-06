using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Reactive.Subjects;

namespace Rush.Azure
{
	internal class QueueReceivingChannel<T> : IReceivingChannel<T>, IObserver<BrokeredMessage>, IDisposable
	{
		private readonly IQueueClient<T> _queueClient;
		private readonly IBrokeredMessageSerializer _brokeredMessageSerializer;
		private readonly ILogger<QueueReceivingChannel<T>> _logger;
		private readonly Subject<T> _subject = new Subject<T>();
		private bool _subscribed = false;
		private readonly object _subscriptionLock = new object();
		private IDisposable _subcription;

		public QueueReceivingChannel(IBrokeredMessageSerializer brokeredMessageSerializer, IQueueClient<T> queueClient, ILogger<QueueReceivingChannel<T>> logger)
		{
			_queueClient = queueClient;
			_brokeredMessageSerializer = brokeredMessageSerializer;
			_logger = logger;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			var subscription = _subject.Subscribe(observer);

			lock (_subscriptionLock)
			{
				if (!_subscribed)
				{
					_subcription = _queueClient.Subscribe(this);
					_subscribed = true;
				}
			}

			return subscription;
		}

		public void OnNext(BrokeredMessage brokeredMessage)
		{
			try
			{
				_subject.OnNext(_brokeredMessageSerializer.Deserialize<T>(brokeredMessage));
			}
			catch (Exception ex)
			{
				_logger.LogError($"An observer of type {nameof(T)} threw an error.", ex);
				throw;
			}
		}

		public void OnError(Exception error) { }

		public void OnCompleted() => _subject.OnCompleted();

		public void Dispose()
		{
			//TODO do we want all of this?
			_subcription?.Dispose();
			OnCompleted();
			_subject.Dispose();
		}
	}
}
