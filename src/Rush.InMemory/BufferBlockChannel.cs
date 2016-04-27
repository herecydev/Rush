﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	public class BufferBlockChannel : ISendingChannel
	{
		public bool Operational => true;
		private readonly ConcurrentDictionary<Type, object> buffers = new ConcurrentDictionary<Type, object>();

		public Task SendAsync<T>(T message, CancellationToken cancellationToken)
		{
			var bufferBlock = buffers.GetOrAdd(typeof(T), _ => new BufferBlock<T>()) as BufferBlock<T>;
			return bufferBlock.SendAsync(message, cancellationToken);
		}
	}
}