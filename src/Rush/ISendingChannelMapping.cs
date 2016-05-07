using System;
using System.Collections.Generic;

namespace Rush
{
    public interface ISendingChannelMapping
    {
		Type Type { get; }
		IEnumerable<ISendingChannel> Channels { get; }
	}
}
