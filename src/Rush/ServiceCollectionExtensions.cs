using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rush
{
	public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRush(this IServiceCollection services)
		{
			services.AddTransient<IMessageStream, MessageSender>();
			services.AddSingleton<IProvideMappings, MappingDictionary>();
			
			return services;
		}

		public static IServiceCollection AddSenderMessageStreams<T>(this IServiceCollection services, IEnumerable<ISendingChannel> messageStreams)
		{
			services.AddInstance<ISenderMessageStreamMapping>(new MessageStreamMapping(typeof(T), messageStreams));

			return services;
		}
    }
}
