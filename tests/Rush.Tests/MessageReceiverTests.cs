using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using TestAttributes;

namespace Rush.Tests
{
	public class MessageReceiverTests
	{
		public class GivenMultipleChannels
		{
			private readonly Mock<IReceivingChannel<string>> _firstChannel;
			private readonly Mock<IReceivingChannel<string>> _secondChannel;
			private readonly Mock<IReceivingChannel<string>> _thirdChannel;
			private readonly Mock<IObserver<string>> _observer;
			private readonly MessageReceiver<string> _messageReceiver;
			private readonly Mock<ILogger<MessageReceiver<string>>> _logger;

			public GivenMultipleChannels()
			{
				_firstChannel = new Mock<IReceivingChannel<string>>();
				_secondChannel = new Mock<IReceivingChannel<string>>();
				_thirdChannel = new Mock<IReceivingChannel<string>>();
				_observer = new Mock<IObserver<string>>();
				_logger = new Mock<ILogger<MessageReceiver<string>>>();

				_messageReceiver = new MessageReceiver<string>(new[] { _firstChannel.Object, _secondChannel.Object, _thirdChannel.Object }, _logger.Object);
			}

			public class WhenSubscribing : GivenMultipleChannels
			{
				[Unit]
				public void ThenLogsInformation()
				{
					_messageReceiver.Subscribe(_observer.Object);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public void ThenSuscribesToEachChannel()
				{
					_messageReceiver.Subscribe(_observer.Object);

					_firstChannel.Verify(x => x.Subscribe(It.IsAny<IObserver<string>>()), Times.Once);
					_secondChannel.Verify(x => x.Subscribe(It.IsAny<IObserver<string>>()), Times.Once);
					_thirdChannel.Verify(x => x.Subscribe(It.IsAny<IObserver<string>>()), Times.Once);
				}
			}

			public class WhenSubscribingWithNullSubscriber : GivenMultipleChannels
			{
				[Unit]
				public void ThenThrowsException()
				{
					Action subscribing = () => _messageReceiver.Subscribe(null);

					subscribing.ShouldThrow<ArgumentNullException>();
				}
			}

			public class WhenDisposing : GivenMultipleChannels
			{
				[Unit]
				public void ThenAllChannelsAreDisposed()
				{
					var disposable = new Mock<IDisposable>();
					_firstChannel.Setup(x => x.Subscribe(It.IsAny<IObserver<string>>())).Returns(disposable.Object);
					_secondChannel.Setup(x => x.Subscribe(It.IsAny<IObserver<string>>())).Returns(disposable.Object);
					_thirdChannel.Setup(x => x.Subscribe(It.IsAny<IObserver<string>>())).Returns(disposable.Object);

					var subscription = _messageReceiver.Subscribe(_observer.Object);

					subscription.Dispose();

					disposable.Verify(x => x.Dispose(), Times.Exactly(3));
				}
			}
		}
	}
}
