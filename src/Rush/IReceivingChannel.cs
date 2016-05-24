using System;

namespace Rush
{
	public interface IReceivingChannel<T> : IObservable<T>
	{
	}
}
