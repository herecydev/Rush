using System;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	internal class BufferBlockDictionary : IBufferBlockDictionary
	{
		private readonly ConcurrentDictionary<Type, object> buffers = new ConcurrentDictionary<Type, object>();

		public BufferBlock<T> GetBufferBlock<T>() => buffers.GetOrAdd(typeof(T), _ => new BufferBlock<T>()) as BufferBlock<T>;
	}
}
