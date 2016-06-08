using Microsoft.Extensions.DependencyInjection;

namespace Rush
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRush(this IServiceCollection services)
		{
			services.AddLogging();
			services.AddTransient(typeof(ISendingStream<>), typeof(UnicastSender<>));
			services.AddTransient(typeof(IReceivingStream<>), typeof(MessageReceiver<>));

			return services;
		}

		public static IServiceCollection WithBroadcastSender<T>(this IServiceCollection services)
		{
			services.AddTransient<ISendingStream<T>, UnicastSender<T>>();

			return services;
		}
	}
}
