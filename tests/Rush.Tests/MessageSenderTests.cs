using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using TestAttributes;

namespace Rush.Tests
{
	public class MessageSenderTests
    {
		private static Task CompletedTask = Task.FromResult(false);
		private const string _message = "Hello world!";

		public class GivenMultipleStreamsAndAllAreOperational
		{
			
			private readonly Mock<ISendingChannel> _firstStream;
			private readonly Mock<ISendingChannel> _secondStream;
			private readonly Mock<ISendingChannel> _thirdStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;
			private readonly Mock<ILogger<MessageSender>> _logger;

			public GivenMultipleStreamsAndAllAreOperational()
			{
				_firstStream = new Mock<ISendingChannel>();
				_firstStream.Setup(x => x.Operational).Returns(true);
				_secondStream = new Mock<ISendingChannel>();
				_secondStream.Setup(x => x.Operational).Returns(true);
				_thirdStream = new Mock<ISendingChannel>();
				_thirdStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _firstStream.Object, _secondStream.Object, _thirdStream.Object };
				_mappingProvider = new Mock<IProvideMappings>();
				_mappingProvider.Setup(x => x.GetSendingChannels<string>()).Returns(streams);

				_logger = new Mock<ILogger<MessageSender>>();

				_messageSender = new MessageSender(_mappingProvider.Object, _logger.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndAllAreOperational
			{
				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public async Task ThenSingleStreamSendsMessage()
				{
					var count = 0;
					_firstStream.Setup(x => x.SendAsync(_message)).Returns(CompletedTask).Callback(() => count++);
					_secondStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);
					_thirdStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);

					await _messageSender.SendAsync(_message);

					count.Should().Be(1);
				}
			}

			public class WhenSendingToSingleStreamWhichFaults : GivenMultipleStreamsAndAllAreOperational
			{
				private int _count;

				public WhenSendingToSingleStreamWhichFaults()
				{
					_count = 0;
					_firstStream.Setup(x => x.SendAsync(_message)).Returns(CompletedTask).Callback(() => { _count++; _firstStream.Setup(x => x.Operational).Returns(false); throw new Exception(); });
					_secondStream.Setup(x => x.SendAsync(_message)).Returns(CompletedTask).Callback(() => _count++);
					_thirdStream.Setup(x => x.SendAsync(_message)).Returns(CompletedTask).Callback(() => _count++);
				}

				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public async Task ThenLogsWarning()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedWarning(Times.Once());
				}

				[Unit]
				public async Task ThenSendsToNextOperationalStream()
				{
					await _messageSender.SendAsync(_message);

					_count.Should().Be(2);
				}
			}
		}

		public class GivenMultipleStreamsAndSomeAreOperational
		{
			private readonly Mock<ISendingChannel> _inoperativeStream;
			private readonly Mock<ISendingChannel> _operationalStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;
			private readonly Mock<ILogger<MessageSender>> _logger;

			public GivenMultipleStreamsAndSomeAreOperational()
			{
				_inoperativeStream = new Mock<ISendingChannel>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_operationalStream = new Mock<ISendingChannel>();
				_operationalStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _inoperativeStream.Object, _operationalStream.Object };
				_mappingProvider = new Mock<IProvideMappings>();
				_mappingProvider.Setup(x => x.GetSendingChannels<string>()).Returns(streams);

				_logger = new Mock<ILogger<MessageSender>>();

				_messageSender = new MessageSender(_mappingProvider.Object, _logger.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndSomeAreOperational
			{
				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public async Task ThenOperationalStreamsSendMessage()
				{
					await _messageSender.SendAsync(_message);

					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_operationalStream.Verify(x => x.SendAsync(_message), Times.Once);
				}
			}
		}

		public class GivenMultipleStreamsAndNoneAreOperational
		{
			private readonly Mock<ISendingChannel> _inoperativeStream;
			private readonly Mock<ISendingChannel> _alternativeInoperativeStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;
			private readonly Mock<ILogger<MessageSender>> _logger;

			public GivenMultipleStreamsAndNoneAreOperational()
			{
				_inoperativeStream = new Mock<ISendingChannel>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_alternativeInoperativeStream = new Mock<ISendingChannel>();
				_alternativeInoperativeStream.Setup(x => x.Operational).Returns(false);
				var streams = new[] { _inoperativeStream.Object, _alternativeInoperativeStream.Object };
				_mappingProvider = new Mock<IProvideMappings>();
				_mappingProvider.Setup(x => x.GetSendingChannels<string>()).Returns(streams);

				_logger = new Mock<ILogger<MessageSender>>();

				_messageSender = new MessageSender(_mappingProvider.Object, _logger.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndNoneAreOperational
			{
				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message).ContinueWith(t => { });

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public void ThenThrowsException()
				{
					Func<Task> sendTask = () => _messageSender.SendAsync(_message);

					sendTask.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("There are no operational message streams.");
					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_alternativeInoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
				}
			}
		}
	}
}
