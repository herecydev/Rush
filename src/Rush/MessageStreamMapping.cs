using System;
using System.Collections.Generic;

namespace Rush
{
	internal class MessageStreamMapping : ISenderMessageStreamMapping
	{
		public Type Type { get; }
		public IEnumerable<ISendingChannel> Streams { get; }

		public MessageStreamMapping(Type type, IEnumerable<ISendingChannel> streams)
		{
			Type = type;
			Streams = streams;
		}
	}
}
