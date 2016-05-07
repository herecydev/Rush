using System;
using System.Collections.Generic;

namespace Rush
{
	internal class MessageChannelMapping : ISendingChannelMapping
	{
		public Type Type { get; }
		public IEnumerable<ISendingChannel> Channels { get; }

		public MessageChannelMapping(Type type, IEnumerable<ISendingChannel> channels)
		{
			Type = type;
			Channels = channels;
		}
	}
}
