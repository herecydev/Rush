using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus.Messaging.Amqp;
using System;
using System.Reactive.Subjects;

namespace Rush.Azure
{
	internal class QueueClient<T> : IQueueClient<T>
	{
		private readonly QueueClient _client;
		private readonly Subject<BrokeredMessage> _subject = new Subject<BrokeredMessage>();

		public QueueClient(IQueueNamer queueNamer, IOptions<AzureMessageOptions<T>> options)
		{
			var queueName = queueNamer.GetQueueName<T>();
			var namespaceManager = NamespaceManager.CreateFromConnectionString(options.Value.ConnectionString);
			if (!namespaceManager.QueueExists(queueName))
				namespaceManager.CreateQueue(new QueueDescription(queueName)
				{

				});
			var connectionBuilder = new ServiceBusConnectionStringBuilder(options.Value.ConnectionString);
			var factory = MessagingFactory.Create(connectionBuilder.EntityPath, new MessagingFactorySettings
			{
				TransportType = TransportType.Amqp,
				TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(connectionBuilder.SharedAccessSignature),
				AmqpTransportSettings = new AmqpTransportSettings
				{
					BatchFlushInterval = options.Value.BatchInterval
				}
			});
			_client = factory.CreateQueueClient(queueName);
		}

		public Task SendAsync(BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return _client.SendAsync(brokeredMessage);
		}

		public IDisposable Subscribe(IObserver<BrokeredMessage> observer)
		{
			var subscription = _subject.Subscribe(observer);
			_client.OnMessage(SendToObservers);

			return subscription;
		}

		private void SendToObservers(BrokeredMessage brokeredMessage)
		{
			try
			{
				_subject.OnNext(brokeredMessage);
				brokeredMessage.Complete();
			}
			catch
			{
				brokeredMessage.Abandon();
			}
		}
	}
}
