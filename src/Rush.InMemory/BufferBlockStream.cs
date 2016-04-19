using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	public class BufferBlockStream : IMessageStream
	{
		public bool Operational => true;
		private readonly ConcurrentDictionary<Type, object> buffers = new ConcurrentDictionary<Type, object>();

		public Task SendAsync<T>(T message)
		{
			var bufferBlock = buffers.GetOrAdd(typeof(T), _ => new BufferBlock<T>()) as BufferBlock<T>;
			return bufferBlock.SendAsync(message);
		}
	}
}