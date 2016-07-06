using Microsoft.Extensions.DependencyInjection;

namespace Rush.Azure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRushAzure(this IServiceCollection services)
		{
			services.AddOptions();
			services.AddTransient(typeof(ISendingChannel<>), typeof(QueueSendingChannel<>));
			services.AddTransient<IBrokeredMessageSerializer, JsonBrokeredMessageSerializer>();
			services.AddTransient<IQueueNamer, JsonBrokeredMessageConverter>();
			services.AddSingleton(typeof(IQueueClient<>), typeof(QueueClient<>));

			return services;
		}

		public static IServiceCollection AddAzureSendingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<ISendingChannel<T>, QueueSendingChannel<T>>();

			return services;
		}

		public static IServiceCollection AddAzureReceivingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<IReceivingChannel<T>, QueueReceivingChannel<T>>();

			return services;
		}
	}
}
