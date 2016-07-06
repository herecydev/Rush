using Microsoft.ServiceBus.Messaging;

namespace Rush.Azure
{
	internal interface IBrokeredMessageSerializer
    {
		BrokeredMessage Serialize<T>(T message);
		T Deserialize<T>(BrokeredMessage brokeredMessage);
	}
}
