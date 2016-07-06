using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rush.Azure
{
	internal interface IQueueClient<T> : IObservable<BrokeredMessage>
	{
		Task SendAsync(BrokeredMessage brokeredMessage, CancellationToken cancellationToken);
	}
}
