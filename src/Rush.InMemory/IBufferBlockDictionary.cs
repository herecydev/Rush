using System.Threading.Tasks.Dataflow;

namespace Rush.InMemory
{
	internal interface IBufferBlockDictionary
	{
		BufferBlock<T> GetBufferBlock<T>();
	}
}
