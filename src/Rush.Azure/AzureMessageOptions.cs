using System;

namespace Rush.Azure
{
	public class AzureMessageOptions<T>
	{
		public string ConnectionString { get; set; }
		public TimeSpan BatchInterval { get; set; }
	}
}