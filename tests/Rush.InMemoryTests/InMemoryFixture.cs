using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rush.InMemoryTests
{
	[CollectionDefinition("InMemory collection")]
	public class InMemoryCollection : ICollectionFixture<InMemoryFixture> { }

	public class InMemoryFixture : IDisposable
    {
		public IServiceProvider ServiceProvider { get; set; }

		public InMemoryFixture()
		{
			var services = new ServiceCollection();

			services.AddRush();
			services.AddRushInMemory();

			ServiceProvider = services.BuildServiceProvider();
		}

		public void Dispose() => (ServiceProvider as IDisposable)?.Dispose();
	}
}
