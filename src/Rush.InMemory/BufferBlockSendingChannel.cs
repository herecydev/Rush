using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	internal class BufferBlockSendingChannel : ISendingChannel
	{
		public bool Operational => true;
		private readonly IBufferBlockDictionary _bufferBlockDictionary;

		public BufferBlockSendingChannel(IBufferBlockDictionary bufferBlockDictionary)
		{
			_bufferBlockDictionary = bufferBlockDictionary;
		}

		public Task SendAsync<T>(T message, CancellationToken cancellationToken)
		{
			var bufferBlock = _bufferBlockDictionary.GetBufferBlock<T>();
			return bufferBlock.SendAsync(message, cancellationToken);
		}
	}
}