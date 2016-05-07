using System.Collections.Generic;

namespace Rush
{
    internal interface IMappingDictionary
    {
		IEnumerable<ISendingChannel> GetSendingChannels<T>();
	}
}
