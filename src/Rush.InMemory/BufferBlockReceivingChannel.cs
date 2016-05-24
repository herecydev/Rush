using System;
using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	internal class BufferBlockReceivingChannel<T> : IReceivingChannel<T>
	{
		private readonly IBufferBlockDictionary _bufferBlockDictionary;

		public BufferBlockReceivingChannel(IBufferBlockDictionary bufferBlockDictionary)
		{
			_bufferBlockDictionary = bufferBlockDictionary;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			var bufferBlock = _bufferBlockDictionary.GetBufferBlock<T>();
			return bufferBlock.AsObservable().Subscribe(observer);
		}
	}
}
