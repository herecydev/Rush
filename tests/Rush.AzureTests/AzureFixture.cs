using Microsoft.Extensions.DependencyInjection;
using Rush.Azure;
using System;
using Xunit;

namespace Rush.AzureTests
{
	[CollectionDefinition("Azure collection")]
	public class AzureCollection : ICollectionFixture<AzureFixture> { }

	public class AzureFixture : IDisposable
	{
		public IServiceProvider ServiceProvider { get; set; }

		public AzureFixture()
		{
			var services = new ServiceCollection();

			services.AddRush();
			services.AddRushAzure();

			ServiceProvider = services.BuildServiceProvider();
		}

		public void Dispose() => (ServiceProvider as IDisposable)?.Dispose();
	}
}
