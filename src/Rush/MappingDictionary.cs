using System.Collections.Generic;
using System.Linq;

namespace Rush
{
	internal class MappingDictionary : IProvideMappings
    {
		private readonly IEnumerable<ISenderMessageStreamMapping> _mappings;
		private readonly IEnumerable<ISendingChannel> _defaultMessageStreams;

		public MappingDictionary(IEnumerable<ISenderMessageStreamMapping> mappings, IEnumerable<ISendingChannel> defaultMessageStreams)
		{
			_mappings = mappings;
			_defaultMessageStreams = defaultMessageStreams;
		}

		public IEnumerable<ISendingChannel> GetSendingChannels<T>() => _mappings.SingleOrDefault(mapping => mapping.Type == typeof(T))?.Streams ?? _defaultMessageStreams;
    }
}
