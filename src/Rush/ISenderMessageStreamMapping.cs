using System;
using System.Collections.Generic;

namespace Rush
{
    public interface ISenderMessageStreamMapping
    {
		Type Type { get; }
		IEnumerable<ISendingChannel> Streams { get; }
	}
}
