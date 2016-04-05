using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rush
{
	public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRush(this IServiceCollection services)
		{
			services.AddTransient(typeof(ISendMessages<>), typeof(MessageSender<>));

			return services;
		}

		public static IServiceCollection AddCustomMessageStreams<T>(this IServiceCollection services, IEnumerable<IMessageStream> messageStreams)
		{
			services.AddTransient<ISendMessages<T>>(serviceProvider => new MessageSender<T>(messageStreams));

			return services;
		}
    }
}
