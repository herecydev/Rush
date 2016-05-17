using Microsoft.Extensions.DependencyInjection;
using Rush.InMemory;

namespace Rush
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRushInMemory(this IServiceCollection services)
		{
			services.AddSingleton<IBufferBlockDictionary, BufferBlockDictionary>();
			services.AddTransient<ISendingChannel, BufferBlockSendingChannel>();
			services.AddTransient(typeof(IReceivingChannel<>), typeof(BufferBlockReceivingChannel<>));

			return services;
		}
	}
}
