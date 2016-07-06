using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rush.Azure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRushAzure(this IServiceCollection services)
		{
			services.AddOptions();
			services.AddTransient(typeof(ISendingChannel<>), typeof(QueueChannel<>));
			services.AddTransient(typeof(IReceivingChannel<>), typeof(QueueChannel<>));
			services.AddTransient<IBrokeredMessageSerializer, JsonBrokeredMessageSerializer>();
			services.AddTransient<IQueueNamer, FakeNamer>();
			services.AddSingleton(typeof(IQueueClient<>), typeof(QueueClient<>));

			//possibly need to use MakeGenericType to create the service.AddTransient(typeof(IOptions<AzureMessagingOptions<>>)); http://stackoverflow.com/questions/7366440/

			return services;
		}

		public static IServiceCollection AddAzureSendingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<ISendingChannel<T>, QueueChannel<T>>();

			return services;
		}

		public static IServiceCollection AddAzureSendingChannel<T>(this IServiceCollection services, AzureMessageOptions<T> options)
		{
			services.AddTransient<ISendingChannel<T>, QueueChannel<T>>();
			services.AddSingleton(Options.Create(options));

			return services;
		}

		public static IServiceCollection AddAzureReceivingChannel<T>(this IServiceCollection services)
		{
			services.AddTransient<IReceivingChannel<T>, QueueChannel<T>>();

			return services;
		}

		public static IServiceCollection AddAzureReceivingChannel<T>(this IServiceCollection services, AzureMessageOptions<T> options)
		{
			services.AddTransient<IReceivingChannel<T>, QueueChannel<T>>();
			services.AddSingleton(Options.Create(options));

			return services;
		}
	}
}
