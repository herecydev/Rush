using System;

namespace Rush.Azure
{
	internal class FakeNamer : IQueueNamer
	{
		public string GetQueueName<T>()
		{
			return "HelloWorld";
		}
	}
}
