using Microsoft.ServiceBus.Messaging;
using System.IO;
using Jil;

namespace Rush.Azure
{
	internal class JsonBrokeredMessageSerializer : IBrokeredMessageSerializer
	{
		public BrokeredMessage Serialize<T>(T message)
		{
			var memoryStream = new MemoryStream();
			using (var streamWriter = new StreamWriter(memoryStream))
				JSON.Serialize(message, streamWriter);

			return new BrokeredMessage(memoryStream, false);
		}

		public T Deserialize<T>(BrokeredMessage brokeredMessage)
		{
			var bodyStream = brokeredMessage.GetBody<Stream>();
			using (var streamReader = new StreamReader(bodyStream))
				return JSON.Deserialize<T>(streamReader);
		}
	}
}
