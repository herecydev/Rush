using Microsoft.ServiceBus.Messaging;
using System;
using System.Reactive.Subjects;

namespace Rush.Azure
{
	internal class QueueReceivingChannel<T> : IReceivingChannel<T>, IObserver<BrokeredMessage>, IDisposable
	{
		private readonly IQueueClient<T> _queueClient;
		private readonly IBrokeredMessageSerializer _brokeredMessageSerializer;
		private readonly Subject<T> _subject = new Subject<T>();
		private object _subscribed = false;
		private IDisposable _subcription;

		public QueueReceivingChannel(IBrokeredMessageSerializer brokeredMessageSerializer, IQueueClient<T> queueClient)
		{
			_queueClient = queueClient;
			_brokeredMessageSerializer = brokeredMessageSerializer;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			var subscription = _subject.Subscribe(observer);

			lock (_subscribed)
			{
				if (!(bool)_subscribed)
				{
					_subcription = _queueClient.Subscribe(this);
					_subscribed = true;
				}
			}

			return subscription;
		}

		public void OnNext(BrokeredMessage value)
		{
			//TODO exceptions + logging
			_subject.OnNext(_brokeredMessageSerializer.Deserialize<T>(value));
		}

		public void OnError(Exception error)
		{
			//Todo log?
			throw new NotImplementedException();
		}

		public void OnCompleted() => _subject.OnCompleted();

		public void Dispose()
		{
			_subcription?.Dispose();
			OnCompleted();
			_subject.Dispose();
		}
	}
}
