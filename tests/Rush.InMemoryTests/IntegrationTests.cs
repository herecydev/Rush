using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;
using Xunit;

namespace Rush.InMemoryTests
{
	public class IntegrationTests
	{
		[Collection("InMemory collection")]
		public class GivenServiceProviderCanResolveSenderAndReceiver
		{
			[Collection("InMemory collection")]
			public class WhenSendingMessage
			{
				private readonly IServiceProvider _serviceProvider;

				public WhenSendingMessage(InMemoryFixture fixture)
				{
					_serviceProvider = fixture.ServiceProvider;
				}

				[Integration]
				public async Task ThenReceiverReceivesMessage()
				{
					var receiver = _serviceProvider.GetRequiredService<IReceivingStream<string>>();
					var semaphore = new SemaphoreSlim(0);
					var sender = _serviceProvider.GetRequiredService<ISendingStream<string>>();
					var observer = new Mock<IObserver<string>>();
					observer.Setup(x => x.OnNext(It.IsAny<string>())).Callback(() => semaphore.Release());
					var message = "Hello world!";

					receiver.Subscribe(observer.Object);
					await sender.SendAsync(message);

					await semaphore.WaitAsync(TimeSpan.FromSeconds(10));

					observer.Verify(x => x.OnNext(message), Times.Once);
				}
			}
		}
	}
}
