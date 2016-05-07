using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rush
{
	public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRush(this IServiceCollection services)
		{
			services.AddTransient<IMessageStream, MessageSender>();
			services.AddTransient(typeof(IReceivingChannel<>), typeof(MessageReceiver<>));
			services.AddTransient<IMappingDictionary, MappingDictionary>();
			
			return services;
		}

		public static IServiceCollection AddSenderMessageChannels<T>(this IServiceCollection services, IEnumerable<ISendingChannel> messageChannels)
		{
			services.AddInstance<ISendingChannelMapping>(new MessageChannelMapping(typeof(T), messageChannels));

			return services;
		}

		public static IServiceCollection AddReceiverMessageChannels<T>(this IServiceCollection services, IEnumerable<IReceivingChannel<T>> messageChannels)
		{
			services.AddInstance(messageChannels);

			return services;
		}
	}
}
