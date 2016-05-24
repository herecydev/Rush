using Microsoft.Extensions.DependencyInjection;
using Rush.InMemory;

namespace Rush
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRushInMemory(this IServiceCollection services)
		{
			services.AddSingleton<IBufferBlockDictionary, BufferBlockDictionary>();
			services.AddTransient(typeof(ISendingChannel<>), typeof(BufferBlockSendingChannel<>));
			services.AddTransient(typeof(IReceivingChannel<>), typeof(BufferBlockReceivingChannel<>));

			return services;
		}

		public static IServiceCollection AddInMemorySendingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<ISendingChannel<T>, BufferBlockSendingChannel<T>>();

			return services;
		}

		public static IServiceCollection AddInMemoryReceivingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<IReceivingChannel<T>, BufferBlockReceivingChannel<T>>();

			return services;
		}
	}
}
