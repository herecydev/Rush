using Microsoft.Extensions.DependencyInjection;

namespace Rush.InMemory
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
