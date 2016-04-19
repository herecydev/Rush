using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using TestAttributes;

namespace Rush.Tests
{
	public class MessageSenderTests
    {
		public static Task CompletedTask = Task.FromResult(false);

		public class GivenMultipleStreamsAndAllAreOperational
		{
			protected const string _message = "Hello world!";
			protected readonly Mock<ISendingChannel> _firstStream;
			protected readonly Mock<ISendingChannel> _secondStream;
			protected readonly Mock<ISendingChannel> _thirdStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;

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

				_messageSender = new MessageSender(_mappingProvider.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndAllAreOperational
			{
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
				[Unit]
				public async Task ThenSendsToNextOperationalStream()
				{
					var count = 0;
					_firstStream.Setup(x => x.SendAsync(_message)).Returns(CompletedTask).Callback(() => count++);
					_secondStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);
					_thirdStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(CompletedTask).Callback(() => count++);

					await _messageSender.SendAsync(_message);

					count.Should().Be(1);
				}
			}
		}

		public class GivenMultipleStreamsAndSomeAreOperational
		{
			protected readonly Mock<ISendingChannel> _inoperativeStream;
			protected const string _message = "Hello world!";
			protected readonly Mock<ISendingChannel> _operationalStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;

			public GivenMultipleStreamsAndSomeAreOperational()
			{
				_inoperativeStream = new Mock<ISendingChannel>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_operationalStream = new Mock<ISendingChannel>();
				_operationalStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _inoperativeStream.Object, _operationalStream.Object };
				_mappingProvider = new Mock<IProvideMappings>();
				_mappingProvider.Setup(x => x.GetSendingChannels<string>()).Returns(streams);

				_messageSender = new MessageSender(_mappingProvider.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndSomeAreOperational
			{
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
			protected readonly Mock<ISendingChannel> _inoperativeStream;
			protected const string _message = "Hello world!";
			protected readonly Mock<ISendingChannel> _alternativeInoperativeStream;
			private readonly MessageSender _messageSender;
			private readonly Mock<IProvideMappings> _mappingProvider;

			public GivenMultipleStreamsAndNoneAreOperational()
			{
				_inoperativeStream = new Mock<ISendingChannel>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_alternativeInoperativeStream = new Mock<ISendingChannel>();
				_alternativeInoperativeStream.Setup(x => x.Operational).Returns(false);
				var streams = new[] { _inoperativeStream.Object, _alternativeInoperativeStream.Object };
				_mappingProvider = new Mock<IProvideMappings>();
				_mappingProvider.Setup(x => x.GetSendingChannels<string>()).Returns(streams);

				_messageSender = new MessageSender(_mappingProvider.Object);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndNoneAreOperational
			{
				[Unit]
				public void ThenOperationalStreamsSendMessage()
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
