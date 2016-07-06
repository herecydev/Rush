using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rush.Azure
{
	internal class QueueSendingChannel<T> : ISendingChannel<T>
	{
		private readonly IQueueClient<T> _queueClient;
		private readonly IBrokeredMessageSerializer _brokeredMessageSerializer;

		public bool Operational
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public QueueSendingChannel(IBrokeredMessageSerializer brokeredMessageSerializer, IQueueClient<T> queueClient)
		{
			_queueClient = queueClient;
			_brokeredMessageSerializer = brokeredMessageSerializer;
		}

		public Task SendAsync(T message, CancellationToken cancellationToken)
		{
			var brokeredMessage = _brokeredMessageSerializer.Serialize(message);
			return _queueClient.SendAsync(brokeredMessage, cancellationToken);
		}
	}
}
