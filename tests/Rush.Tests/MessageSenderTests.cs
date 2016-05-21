using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;

namespace Rush.Tests
{
	public class MessageSenderTests
    {
		private static Task CompletedTask = Task.FromResult(false);
		private const string _message = "Hello world!";

		public class GivenMultipleChannelsAndAllAreOperational
		{
			private readonly Mock<ISendingChannel<string>> _firstChannel;
			private readonly Mock<ISendingChannel<string>> _secondChannel;
			private readonly Mock<ISendingChannel<string>> _thirdChannel;
			private readonly MessageSender<string> _messageSender;
			private readonly Mock<ILogger<MessageSender<string>>> _logger;

			public GivenMultipleChannelsAndAllAreOperational()
			{
				_firstChannel = new Mock<ISendingChannel<string>>();
				_firstChannel.Setup(x => x.Operational).Returns(true);
				_secondChannel = new Mock<ISendingChannel<string>>();
				_secondChannel.Setup(x => x.Operational).Returns(true);
				_thirdChannel = new Mock<ISendingChannel<string>>();
				_thirdChannel.Setup(x => x.Operational).Returns(true);

				_logger = new Mock<ILogger<MessageSender<string>>>();

				_messageSender = new MessageSender<string>(new[] { _firstChannel.Object, _secondChannel.Object, _thirdChannel.Object }, _logger.Object);
			}

			public class WhenSending : GivenMultipleChannelsAndAllAreOperational
			{
				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public async Task ThenSingleChannelSendsMessage()
				{
					var count = 0;
					_firstChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Returns(CompletedTask).Callback(() => count++);
					_secondChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);
					_thirdChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);

					await _messageSender.SendAsync(_message);

					count.Should().Be(1);
				}
			}

			public class WhenSendingToSingleChannelWhichFaults : GivenMultipleChannelsAndAllAreOperational
			{
				private int _count;

				public WhenSendingToSingleChannelWhichFaults()
				{
					_count = 0;
					_firstChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Returns(CompletedTask).Callback(() => { _count++; _firstChannel.Setup(x => x.Operational).Returns(false); throw new Exception(); });
					_secondChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Returns(CompletedTask).Callback(() => _count++);
					_thirdChannel.Setup(x => x.SendAsync(_message, CancellationToken.None)).Returns(CompletedTask).Callback(() => _count++);
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
				public async Task ThenSendsToNextOperationalChannel()
				{
					await _messageSender.SendAsync(_message);

					_count.Should().Be(2);
				}
			}
		}

		public class GivenMultipleChannelsAndSomeAreOperational
		{
			private readonly Mock<ISendingChannel<string>> _inoperativeChannel;
			private readonly Mock<ISendingChannel<string>> _operationalChannel;
			private readonly MessageSender<string> _messageSender;
			private readonly Mock<ILogger<MessageSender<string>>> _logger;

			public GivenMultipleChannelsAndSomeAreOperational()
			{
				_inoperativeChannel = new Mock<ISendingChannel<string>>();
				_inoperativeChannel.Setup(x => x.Operational).Returns(false);
				_operationalChannel = new Mock<ISendingChannel<string>>();
				_operationalChannel.Setup(x => x.Operational).Returns(true);

				_logger = new Mock<ILogger<MessageSender<string>>>();

				_messageSender = new MessageSender<string>(new[] { _inoperativeChannel.Object, _operationalChannel.Object }, _logger.Object);
			}

			public class WhenSending : GivenMultipleChannelsAndSomeAreOperational
			{
				[Unit]
				public async Task ThenLogsInformation()
				{
					await _messageSender.SendAsync(_message);

					_logger.VerifyLoggedInformation(Times.Once());
				}

				[Unit]
				public async Task ThenOperationalChannelsSendMessage()
				{
					await _messageSender.SendAsync(_message);

					_inoperativeChannel.Verify(x => x.SendAsync(_message, CancellationToken.None), Times.Never);
					_operationalChannel.Verify(x => x.SendAsync(_message, CancellationToken.None), Times.Once);
				}
			}
		}

		public class GivenMultipleChannelsAndNoneAreOperational
		{
			private readonly Mock<ISendingChannel<string>> _inoperativeChannel;
			private readonly Mock<ISendingChannel<string>> _alternativeInoperativeChannel;
			private readonly MessageSender<string> _messageSender;
			private readonly Mock<ILogger<MessageSender<string>>> _logger;

			public GivenMultipleChannelsAndNoneAreOperational()
			{
				_inoperativeChannel = new Mock<ISendingChannel<string>>();
				_inoperativeChannel.Setup(x => x.Operational).Returns(false);
				_alternativeInoperativeChannel = new Mock<ISendingChannel<string>>();
				_alternativeInoperativeChannel.Setup(x => x.Operational).Returns(false);

				_logger = new Mock<ILogger<MessageSender<string>>>();

				_messageSender = new MessageSender<string>(new[] { _inoperativeChannel.Object, _alternativeInoperativeChannel.Object }, _logger.Object);
			}

			public class WhenSending : GivenMultipleChannelsAndNoneAreOperational
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

					sendTask.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("There are no operational message channels.");
					_inoperativeChannel.Verify(x => x.SendAsync(_message, CancellationToken.None), Times.Never);
					_alternativeInoperativeChannel.Verify(x => x.SendAsync(_message, CancellationToken.None), Times.Never);
				}
			}
		}
	}
}
