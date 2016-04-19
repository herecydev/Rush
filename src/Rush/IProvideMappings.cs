using System.Collections.Generic;

namespace Rush
{
    public interface IProvideMappings
    {
		IEnumerable<ISendingChannel> GetSendingChannels<T>();
	}
}
