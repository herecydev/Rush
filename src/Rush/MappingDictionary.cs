using System.Collections.Generic;
using System.Linq;

namespace Rush
{
	internal class MappingDictionary : IMappingDictionary
    {
		private readonly IEnumerable<ISendingChannelMapping> _sendingMappings;
		private readonly IEnumerable<ISendingChannel> _defaultChannels;

		public MappingDictionary(IEnumerable<ISendingChannelMapping> sendingMappings, IEnumerable<ISendingChannel> defaultChannels)
		{
			_sendingMappings = sendingMappings;
			_defaultChannels = defaultChannels;
		}

		public IEnumerable<ISendingChannel> GetSendingChannels<T>() => _sendingMappings.SingleOrDefault(mapping => mapping.Type == typeof(T))?.Channels ?? _defaultChannels;
	}
}
