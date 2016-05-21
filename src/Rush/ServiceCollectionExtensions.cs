using Microsoft.Extensions.DependencyInjection;

namespace Rush
{
	public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRush(this IServiceCollection services)
		{
			services.AddLogging();
			services.AddTransient(typeof(ISendingStream<>), typeof(MessageSender<>));
			services.AddTransient(typeof(IReceivingStream<>), typeof(MessageReceiver<>));
			
			return services;
		}
	}
}
