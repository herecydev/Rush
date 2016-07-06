using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Rush.Azure
{
	internal class QueueChannel<T> : IReceivingChannel<T>, ISendingChannel<T>, IObserver<BrokeredMessage>, IDisposable
	{
		private readonly IQueueClient<T> _queueClient;
		private readonly IBrokeredMessageSerializer _brokeredMessageSerializer;
		private readonly ILogger<QueueChannel<T>> _logger;
		private readonly Subject<T> _subject = new Subject<T>();
		private bool _subscribed = false;
		private readonly object _subscriptionLock = new object();
		private IDisposable _subcription;

		public bool Operational { get; set; }

		public QueueChannel(IBrokeredMessageSerializer brokeredMessageSerializer, IQueueClient<T> queueClient, ILogger<QueueChannel<T>> logger)
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

		public Task SendAsync(T message, CancellationToken cancellationToken)
		{
			var brokeredMessage = _brokeredMessageSerializer.Serialize(message);
			return _queueClient.SendAsync(brokeredMessage, cancellationToken);
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

		public void OnError(Exception error)
		{
			_logger.LogError($"Queue client of type {nameof(T)} threw an error.", error);
			Operational = false;
		}

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
