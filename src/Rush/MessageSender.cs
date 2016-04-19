using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rush
{
	internal class MessageSender : IMessageStream
	{
		private readonly IProvideMappings _mappingDictionary;

		public MessageSender(IProvideMappings mappingDictionary)
		{
			_mappingDictionary = mappingDictionary;
		}

		public async Task SendAsync<T>(T message)
		{
			var channels = _mappingDictionary.GetSendingChannels<T>();
			var operationalStream = channels.FirstOrDefault(stream => stream.Operational);

			if (operationalStream == null)
				throw new InvalidOperationException("There are no operational message streams.");

			try
			{
				await operationalStream.SendAsync(message);
			}
			catch (Exception)
			{
				await SendAsync(message);
			}
		}
	}
}