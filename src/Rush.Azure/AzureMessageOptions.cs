using System;

namespace Rush.Azure
{
	public class AzureMessageOptions
	{
		public string ConnectionString { get; set; }
		public TimeSpan BatchInterval { get; set; }
	}

	public class AzureMessageOptions<T> : AzureMessageOptions
	{

	}
}