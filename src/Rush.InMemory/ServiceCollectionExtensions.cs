using Microsoft.Extensions.DependencyInjection;

namespace Rush.InMemory
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRushInMemory(this IServiceCollection services)
		{
			services.AddTransient<IMessageStream, BufferBlockStream>();

			return services;
		}
	}
}
